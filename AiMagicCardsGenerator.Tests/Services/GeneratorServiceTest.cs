using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AiMagicCardsGenerator.Models.Entities;
using AiMagicCardsGenerator.Repositories;
using AiMagicCardsGenerator.Services;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Microsoft.Extensions.Configuration;

namespace AiMagicCardsGenerator.Tests.Services;

[TestClass]
[TestSubject(typeof(GeneratorService))]
public class GeneratorServiceTest {
    private Mock<ICardRepository>    _mockRepository    = null!;
    private Mock<HttpMessageHandler> _mockHttpHandler   = null!;
    private Mock<IConfiguration>     _mockConfiguration = null!;
    private HttpClient               _httpClient        = null!;
    private GeneratorService         _service           = null!;

    [TestInitialize]
    public void Setup() {
        _mockRepository    = new Mock<ICardRepository>();
        _mockHttpHandler   = new Mock<HttpMessageHandler>();
        _mockConfiguration = new Mock<IConfiguration>();

        _mockConfiguration.Setup(c => c["Groq:ApiKey"]).Returns("test-api-key");

        _httpClient = new HttpClient(_mockHttpHandler.Object);
        _service    = new GeneratorService(_mockRepository.Object, _httpClient, _mockConfiguration.Object);
    }

    #region GenerateRandomCardAsync

    [TestMethod]
    public async Task GenerateRandomCardAsync_ValidResponse_ReturnsCard() {
        // Arrange
        var examples = CreateExampleCards(3);
        _mockRepository.Setup(r => r.GetRandomByCmcAsync(It.IsAny<int>(), 5)).ReturnsAsync(examples);
        SetupHttpResponse(CreateValidGroqResponse("Generated Card", "{2}{R}", "Creature - Goblin", "Haste", "2", "1"));

        // Act
        var result = await _service.GenerateRandomCardAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Card);
        Assert.AreEqual("Generated Card", result.Card.Name);
    }

    [TestMethod]
    public async Task GenerateRandomCardAsync_ReturnsBasedOnCards() {
        // Arrange
        var examples = CreateExampleCards(5);
        _mockRepository.Setup(r => r.GetRandomByCmcAsync(It.IsAny<int>(), 5)).ReturnsAsync(examples);
        SetupHttpResponse(CreateValidGroqResponse("Test", "{R}", "Instant", "Deal 3 damage"));

        // Act
        var result = await _service.GenerateRandomCardAsync();

        // Assert
        Assert.IsNotNull(result.BasedOn);
        Assert.AreEqual(5, result.BasedOn.Count);
    }

    [TestMethod]
    public async Task GenerateRandomCardAsync_CallsRepositoryWithCorrectTake() {
        // Arrange
        _mockRepository.Setup(r => r.GetRandomByCmcAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<Card>());
        SetupHttpResponse(CreateValidGroqResponse("Test", "{R}", "Instant", "Text"));

        // Act
        await _service.GenerateRandomCardAsync();

        // Assert
        _mockRepository.Verify(r => r.GetRandomByCmcAsync(It.IsAny<int>(), 5), Times.Once);
    }

    [TestMethod]
    public async Task GenerateRandomCardAsync_CreatureCard_ParsesPowerToughness() {
        // Arrange
        _mockRepository.Setup(r => r.GetRandomByCmcAsync(It.IsAny<int>(), 5)).ReturnsAsync(new List<Card>());
        SetupHttpResponse(CreateValidGroqResponse("Big Creature", "{4}{G}{G}", "Creature - Beast", "Trample", "6",
            "6"));

        // Act
        var result = await _service.GenerateRandomCardAsync();

        // Assert
        Assert.AreEqual("6", result.Card.Power);
        Assert.AreEqual("6", result.Card.Toughness);
    }

    [TestMethod]
    public async Task GenerateRandomCardAsync_NonCreatureCard_NullPowerToughness() {
        // Arrange
        _mockRepository.Setup(r => r.GetRandomByCmcAsync(It.IsAny<int>(), 5)).ReturnsAsync(new List<Card>());
        SetupHttpResponse(CreateValidGroqResponse("Lightning Bolt", "{R}", "Instant", "Deal 3 damage", null, null));

        // Act
        var result = await _service.GenerateRandomCardAsync();

        // Assert
        Assert.IsNull(result.Card.Power);
        Assert.IsNull(result.Card.Toughness);
    }

    [TestMethod]
    public async Task GenerateRandomCardAsync_ParsesFlavorText() {
        // Arrange
        _mockRepository.Setup(r => r.GetRandomByCmcAsync(It.IsAny<int>(), 5)).ReturnsAsync(new List<Card>());
        var response = CreateGroqResponseWithFlavor("Test Card", "{1}", "Artifact", "Tap: Add {C}", "Ancient relic");
        SetupHttpResponse(response);

        // Act
        var result = await _service.GenerateRandomCardAsync();

        // Assert
        Assert.AreEqual("Ancient relic", result.Card.FlavorText);
    }

    #endregion

    #region API Error Handling

    [TestMethod]
    public async Task GenerateRandomCardAsync_MissingApiKey_ThrowsException() {
        // Arrange
        _mockConfiguration.Setup(c => c["Groq:ApiKey"]).Returns((string?)null);
        _mockRepository.Setup(r => r.GetRandomByCmcAsync(It.IsAny<int>(), 5)).ReturnsAsync(new List<Card>());

        var service = new GeneratorService(_mockRepository.Object, _httpClient, _mockConfiguration.Object);

        // Act & Assert
        try {
            await service.GenerateRandomCardAsync();
            Assert.Fail("Expected InvalidOperationException was not thrown");
        }
        catch (InvalidOperationException) {
            // Expected exception
        }
    }

    [TestMethod]
    public async Task GenerateRandomCardAsync_MissingApiKey_ExceptionContainsMessage() {
        // Arrange
        _mockConfiguration.Setup(c => c["Groq:ApiKey"]).Returns((string?)null);
        _mockRepository.Setup(r => r.GetRandomByCmcAsync(It.IsAny<int>(), 5)).ReturnsAsync(new List<Card>());

        var service = new GeneratorService(_mockRepository.Object, _httpClient, _mockConfiguration.Object);

        // Act & Assert
        try {
            await service.GenerateRandomCardAsync();
            Assert.Fail("Expected InvalidOperationException was not thrown");
        }
        catch (InvalidOperationException ex) {
            Assert.IsTrue(ex.Message.Contains("API key"));
        }
    }

    [TestMethod]
    public async Task GenerateRandomCardAsync_ApiReturns500_ThrowsException() {
        // Arrange
        _mockRepository.Setup(r => r.GetRandomByCmcAsync(It.IsAny<int>(), 5)).ReturnsAsync(new List<Card>());
        SetupHttpResponse("Internal Server Error", HttpStatusCode.InternalServerError);

        // Act & Assert
        try {
            await _service.GenerateRandomCardAsync();
            Assert.Fail("Expected InvalidOperationException was not thrown");
        }
        catch (InvalidOperationException) {
            // Expected exception
        }
    }

    [TestMethod]
    public async Task GenerateRandomCardAsync_ApiReturns401_ThrowsException() {
        // Arrange
        _mockRepository.Setup(r => r.GetRandomByCmcAsync(It.IsAny<int>(), 5)).ReturnsAsync(new List<Card>());
        SetupHttpResponse("Unauthorized", HttpStatusCode.Unauthorized);

        // Act & Assert
        try {
            await _service.GenerateRandomCardAsync();
            Assert.Fail("Expected InvalidOperationException was not thrown");
        }
        catch (InvalidOperationException) {
            // Expected exception
        }
    }

    [TestMethod]
    public async Task GenerateRandomCardAsync_ApiError_ExceptionContainsStatusCode() {
        // Arrange
        _mockRepository.Setup(r => r.GetRandomByCmcAsync(It.IsAny<int>(), 5)).ReturnsAsync(new List<Card>());
        SetupHttpResponse("Rate limited", HttpStatusCode.TooManyRequests);

        // Act & Assert
        try {
            await _service.GenerateRandomCardAsync();
            Assert.Fail("Expected InvalidOperationException was not thrown");
        }
        catch (InvalidOperationException ex) {
            Assert.IsTrue(ex.Message.Contains("429") || ex.Message.Contains("TooManyRequests"));
        }
    }

    #endregion

    #region Response Parsing

    [TestMethod]
    public async Task GenerateRandomCardAsync_InvalidJson_ThrowsException() {
        // Arrange
        _mockRepository.Setup(r => r.GetRandomByCmcAsync(It.IsAny<int>(), 5)).ReturnsAsync(new List<Card>());
        SetupHttpResponse(CreateGroqResponseRaw("This is not JSON at all"));

        // Act & Assert
        try {
            await _service.GenerateRandomCardAsync();
            Assert.Fail("Expected InvalidOperationException was not thrown");
        }
        catch (InvalidOperationException) {
            // Expected exception
        }
    }

    [TestMethod]
    public async Task GenerateRandomCardAsync_NoJsonBraces_ThrowsException() {
        // Arrange
        _mockRepository.Setup(r => r.GetRandomByCmcAsync(It.IsAny<int>(), 5)).ReturnsAsync(new List<Card>());
        SetupHttpResponse(CreateGroqResponseRaw("Here is the card: name is Test, cost is R"));

        // Act & Assert
        try {
            await _service.GenerateRandomCardAsync();
            Assert.Fail("Expected InvalidOperationException was not thrown");
        }
        catch (InvalidOperationException) {
            // Expected exception
        }
    }

    [TestMethod]
    public async Task GenerateRandomCardAsync_JsonWithExtraText_ParsesCorrectly() {
        // Arrange
        _mockRepository.Setup(r => r.GetRandomByCmcAsync(It.IsAny<int>(), 5)).ReturnsAsync(new List<Card>());
        var jsonWithPreamble = "Here is your card:\n" + CreateCardJson("Extracted Card", "{1}{U}", "Instant", "Draw");
        SetupHttpResponse(CreateGroqResponseRaw(jsonWithPreamble));

        // Act
        var result = await _service.GenerateRandomCardAsync();

        // Assert
        Assert.AreEqual("Extracted Card", result.Card.Name);
    }

    [TestMethod]
    public async Task GenerateRandomCardAsync_JsonWithMarkdownCodeBlock_ParsesCorrectly() {
        // Arrange
        _mockRepository.Setup(r => r.GetRandomByCmcAsync(It.IsAny<int>(), 5)).ReturnsAsync(new List<Card>());
        var jsonWithMarkdown = "```json\n" + CreateCardJson("Markdown Card", "{2}", "Artifact", "Tap") + "\n```";
        SetupHttpResponse(CreateGroqResponseRaw(jsonWithMarkdown));

        // Act
        var result = await _service.GenerateRandomCardAsync();

        // Assert
        Assert.AreEqual("Markdown Card", result.Card.Name);
    }

    #endregion

    #region HTTP Request Verification

    [TestMethod]
    public async Task GenerateRandomCardAsync_SendsAuthorizationHeader() {
        // Arrange
        _mockRepository.Setup(r => r.GetRandomByCmcAsync(It.IsAny<int>(), 5)).ReturnsAsync(new List<Card>());

        HttpRequestMessage? capturedRequest = null;
        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content    = new StringContent(CreateValidGroqResponse("Test", "{R}", "Instant", "Text"))
            });

        // Act
        await _service.GenerateRandomCardAsync();

        // Assert
        Assert.IsNotNull(capturedRequest);
        Assert.IsTrue(capturedRequest.Headers.Contains("Authorization"));
        Assert.AreEqual("Bearer test-api-key", capturedRequest.Headers.GetValues("Authorization").First());
    }

    [TestMethod]
    public async Task GenerateRandomCardAsync_SendsToCorrectEndpoint() {
        // Arrange
        _mockRepository.Setup(r => r.GetRandomByCmcAsync(It.IsAny<int>(), 5)).ReturnsAsync(new List<Card>());

        HttpRequestMessage? capturedRequest = null;
        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content    = new StringContent(CreateValidGroqResponse("Test", "{R}", "Instant", "Text"))
            });

        // Act
        await _service.GenerateRandomCardAsync();

        // Assert
        Assert.IsNotNull(capturedRequest);
        Assert.AreEqual("https://api.groq.com/openai/v1/chat/completions", capturedRequest.RequestUri?.ToString());
    }

    [TestMethod]
    public async Task GenerateRandomCardAsync_SendsPostRequest() {
        // Arrange
        _mockRepository.Setup(r => r.GetRandomByCmcAsync(It.IsAny<int>(), 5)).ReturnsAsync(new List<Card>());

        HttpRequestMessage? capturedRequest = null;
        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content    = new StringContent(CreateValidGroqResponse("Test", "{R}", "Instant", "Text"))
            });

        // Act
        await _service.GenerateRandomCardAsync();

        // Assert
        Assert.IsNotNull(capturedRequest);
        Assert.AreEqual(HttpMethod.Post, capturedRequest.Method);
    }

    [TestMethod]
    public async Task GenerateRandomCardAsync_RequestContainsModel() {
        // Arrange
        _mockRepository.Setup(r => r.GetRandomByCmcAsync(It.IsAny<int>(), 5)).ReturnsAsync(new List<Card>());

        string? capturedBody = null;
        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>(async (req, _) => {
                capturedBody = await req.Content!.ReadAsStringAsync();
            })
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content    = new StringContent(CreateValidGroqResponse("Test", "{R}", "Instant", "Text"))
            });

        // Act
        await _service.GenerateRandomCardAsync();

        // Assert
        Assert.IsNotNull(capturedBody);
        Assert.IsTrue(capturedBody.Contains("llama-3.3-70b-versatile"));
    }

    #endregion

    #region Helpers

    private void SetupHttpResponse(string content, HttpStatusCode statusCode = HttpStatusCode.OK) {
        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = statusCode,
                Content    = new StringContent(content, Encoding.UTF8, "application/json")
            });
    }

    private static List<Card> CreateExampleCards(int count) {
        return Enumerable.Range(1, count).Select(i => new Card {
            Id         = i,
            Name       = $"Example Card {i}",
            ManaCost   = "{1}{R}",
            TypeLine   = "Creature - Goblin",
            OracleText = "Haste",
            Power      = "2",
            Toughness  = "1"
        }).ToList();
    }

    private static string CreateValidGroqResponse(
        string  name,
        string  manaCost,
        string  typeLine,
        string  oracleText,
        string? power     = null,
        string? toughness = null) {
        var cardJson = CreateCardJson(name, manaCost, typeLine, oracleText, power, toughness);
        return CreateGroqResponseRaw(cardJson);
    }

    private static string CreateGroqResponseWithFlavor(
        string name,
        string manaCost,
        string typeLine,
        string oracleText,
        string flavorText) {
        var card = new {
            name,
            manaCost,
            typeLine,
            oracleText,
            power     = (string?)null,
            toughness = (string?)null,
            flavorText
        };
        return CreateGroqResponseRaw(JsonSerializer.Serialize(card));
    }

    private static string CreateCardJson(
        string  name,
        string  manaCost,
        string  typeLine,
        string  oracleText,
        string? power     = null,
        string? toughness = null) {
        var card = new {
            name,
            manaCost,
            typeLine,
            oracleText,
            power,
            toughness,
            flavorText = "Test flavor"
        };
        return JsonSerializer.Serialize(card);
    }

    private static string CreateGroqResponseRaw(string content) {
        var response = new {
            choices = new[] {
                new {
                    message = new {
                        content
                    }
                }
            }
        };
        return JsonSerializer.Serialize(response);
    }

    #endregion
}