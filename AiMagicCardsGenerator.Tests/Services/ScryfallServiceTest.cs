using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AiMagicCardsGenerator.Models.Dto;
using AiMagicCardsGenerator.Services;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace AiMagicCardsGenerator.Tests.Services;

[TestClass]
[TestSubject(typeof(ScryfallService))]
public class ScryfallServiceTest {

    private Mock<HttpMessageHandler> _mockHttpHandler = null!;
    private HttpClient               _httpClient      = null!;
    private ScryfallService          _service         = null!;

    [TestInitialize]
    public void Setup() {
        _mockHttpHandler = new Mock<HttpMessageHandler>();
        _httpClient      = new HttpClient(_mockHttpHandler.Object) {
            BaseAddress = new Uri("https://api.scryfall.com/")
        };
        _service = new ScryfallService(_httpClient);
    }

    #region GetBulkDataInfoAsync

    [TestMethod]
    public async Task GetBulkDataInfoAsync_ValidResponse_ReturnsAllCardsItem() {
        // Arrange
        var bulkDataResponse = CreateBulkDataResponse("all_cards", "https://data.scryfall.io/all-cards.json");
        SetupHttpResponse("bulk-data", bulkDataResponse);

        // Act
        var result = await _service.GetBulkDataInfoAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("all_cards", result.Type);
    }

    [TestMethod]
    public async Task GetBulkDataInfoAsync_ValidResponse_ReturnsCorrectDownloadUri() {
        // Arrange
        var expectedUri = "https://data.scryfall.io/all-cards-12345.json";
        var bulkDataResponse = CreateBulkDataResponse("all_cards", expectedUri);
        SetupHttpResponse("bulk-data", bulkDataResponse);

        // Act
        var result = await _service.GetBulkDataInfoAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedUri, result.DownloadUri);
    }

    [TestMethod]
    public async Task GetBulkDataInfoAsync_MultipleTypes_ReturnsOnlyAllCards() {
        // Arrange
        var response = new ScryfallBulkDataResponse {
            Data = new List<ScryfallBulkDataItem> {
                new() { Type = "oracle_cards",  DownloadUri = "https://oracle.json" },
                new() { Type = "all_cards",     DownloadUri = "https://all.json" },
                new() { Type = "default_cards", DownloadUri = "https://default.json" }
            }
        };
        SetupHttpResponse("bulk-data", JsonSerializer.Serialize(response));

        // Act
        var result = await _service.GetBulkDataInfoAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("all_cards", result.Type);
        Assert.AreEqual("https://all.json", result.DownloadUri);
    }

    [TestMethod]
    public async Task GetBulkDataInfoAsync_NoAllCardsType_ReturnsNull() {
        // Arrange
        var response = new ScryfallBulkDataResponse {
            Data = new List<ScryfallBulkDataItem> {
                new() { Type = "oracle_cards",  DownloadUri = "https://oracle.json" },
                new() { Type = "default_cards", DownloadUri = "https://default.json" }
            }
        };
        SetupHttpResponse("bulk-data", JsonSerializer.Serialize(response));

        // Act
        var result = await _service.GetBulkDataInfoAsync();

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetBulkDataInfoAsync_EmptyData_ReturnsNull() {
        // Arrange
        var response = new ScryfallBulkDataResponse { Data = new List<ScryfallBulkDataItem>() };
        SetupHttpResponse("bulk-data", JsonSerializer.Serialize(response));

        // Act
        var result = await _service.GetBulkDataInfoAsync();

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetBulkDataInfoAsync_CallsCorrectEndpoint() {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        SetupHttpResponseWithCapture("bulk-data", CreateBulkDataResponse("all_cards", "https://test.json"), req => capturedRequest = req);

        // Act
        await _service.GetBulkDataInfoAsync();

        // Assert
        Assert.IsNotNull(capturedRequest);
        Assert.Contains("bulk-data", capturedRequest.RequestUri!.ToString());
    }

    #endregion

    #region DownloadAllCardsAsync

    [TestMethod]
    public async Task DownloadAllCardsAsync_ValidResponse_ReturnsCards() {
        // Arrange
        SetupBulkDataAndCardsResponse(new List<ScryfallCard> {
            CreateScryfallCard("Lightning Bolt"),
            CreateScryfallCard("Giant Growth")
        });

        // Act
        var result = await _service.DownloadAllCardsAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(2, result);
    }

    [TestMethod]
    public async Task DownloadAllCardsAsync_ValidResponse_ParsesCardNames() {
        // Arrange
        SetupBulkDataAndCardsResponse(new List<ScryfallCard> {
            CreateScryfallCard("Lightning Bolt"),
            CreateScryfallCard("Giant Growth")
        });

        // Act
        var result = await _service.DownloadAllCardsAsync();

        // Assert
        Assert.AreEqual("Lightning Bolt", result[0].Name);
        Assert.AreEqual("Giant Growth", result[1].Name);
    }

    [TestMethod]
    public async Task DownloadAllCardsAsync_ValidResponse_ParsesManaCost() {
        // Arrange
        var card = CreateScryfallCard("Test Card");
        card.ManaCost = "{2}{R}{R}";
        SetupBulkDataAndCardsResponse(new List<ScryfallCard> { card });

        // Act
        var result = await _service.DownloadAllCardsAsync();

        // Assert
        Assert.AreEqual("{2}{R}{R}", result[0].ManaCost);
    }

    [TestMethod]
    public async Task DownloadAllCardsAsync_ValidResponse_ParsesTypeLine() {
        // Arrange
        var card = CreateScryfallCard("Test Creature");
        card.TypeLine = "Creature — Human Wizard";
        SetupBulkDataAndCardsResponse(new List<ScryfallCard> { card });

        // Act
        var result = await _service.DownloadAllCardsAsync();

        // Assert
        Assert.AreEqual("Creature — Human Wizard", result[0].TypeLine);
    }

    [TestMethod]
    public async Task DownloadAllCardsAsync_ValidResponse_ParsesPowerToughness() {
        // Arrange
        var card = CreateScryfallCard("Test Creature");
        card.Power     = "3";
        card.Toughness = "2";
        SetupBulkDataAndCardsResponse(new List<ScryfallCard> { card });

        // Act
        var result = await _service.DownloadAllCardsAsync();

        // Assert
        Assert.AreEqual("3", result[0].Power);
        Assert.AreEqual("2", result[0].Toughness);
    }

    [TestMethod]
    public async Task DownloadAllCardsAsync_EmptyCardList_ReturnsEmptyList() {
        // Arrange
        SetupBulkDataAndCardsResponse(new List<ScryfallCard>());

        // Act
        var result = await _service.DownloadAllCardsAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }

    [TestMethod]
    public async Task DownloadAllCardsAsync_NoBulkDataInfo_ThrowsException() {
        // Arrange
        var response = new ScryfallBulkDataResponse { Data = new List<ScryfallBulkDataItem>() };
        SetupHttpResponse("bulk-data", JsonSerializer.Serialize(response));

        // Act & Assert
        var caught = false;
        try {
            await _service.DownloadAllCardsAsync();
        }
        catch (Exception ex) {
            caught = true;
            Assert.Contains("bulk data", ex.Message);
        }

        Assert.IsTrue(caught);
    }

    [TestMethod]
    public async Task DownloadAllCardsAsync_DownloadFails_ThrowsException() {
        // Arrange
        var bulkDataResponse = CreateBulkDataResponse("all_cards", "https://data.scryfall.io/cards.json");
        SetupHttpResponse("bulk-data", bulkDataResponse);
        SetupHttpResponseForUrl("https://data.scryfall.io/cards.json", "", HttpStatusCode.InternalServerError);

        // Act & Assert
        var caught = false;
        try {
            await _service.DownloadAllCardsAsync();
        }
        catch (HttpRequestException) {
            caught = true;
        }

        Assert.IsTrue(caught);
    }

    [TestMethod]
    public async Task DownloadAllCardsAsync_CallsCorrectDownloadUri() {
        // Arrange
        var downloadUri = "https://data.scryfall.io/all-cards-20240101.json";
        var bulkDataResponse = CreateBulkDataResponse("all_cards", downloadUri);

        HttpRequestMessage? capturedDownloadRequest = null;

        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("bulk-data")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content    = new StringContent(bulkDataResponse, Encoding.UTF8, "application/json")
            });

        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("data.scryfall.io")),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedDownloadRequest = req)
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content    = new StringContent("[]", Encoding.UTF8, "application/json")
            });

        // Act
        await _service.DownloadAllCardsAsync();

        // Assert
        Assert.IsNotNull(capturedDownloadRequest);
        Assert.AreEqual(downloadUri, capturedDownloadRequest.RequestUri!.ToString());
    }

    #endregion

    #region Helpers

    private void SetupHttpResponse(string endpoint, string content) {
        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains(endpoint)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content    = new StringContent(content, Encoding.UTF8, "application/json")
            });
    }

    private void SetupHttpResponseWithCapture(string endpoint, string content, Action<HttpRequestMessage> capture) {
        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains(endpoint)),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => capture(req))
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content    = new StringContent(content, Encoding.UTF8, "application/json")
            });
    }

    private void SetupHttpResponseForUrl(string url, string content, HttpStatusCode statusCode = HttpStatusCode.OK) {
        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString() == url),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = statusCode,
                Content    = new StringContent(content, Encoding.UTF8, "application/json")
            });
    }

    private void SetupBulkDataAndCardsResponse(List<ScryfallCard> cards) {
        var downloadUri = "https://data.scryfall.io/all-cards.json";
        var bulkDataResponse = CreateBulkDataResponse("all_cards", downloadUri);
        var cardsJson = JsonSerializer.Serialize(cards, new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });

        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("bulk-data")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content    = new StringContent(bulkDataResponse, Encoding.UTF8, "application/json")
            });

        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("data.scryfall.io")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content    = new StringContent(cardsJson, Encoding.UTF8, "application/json")
            });
    }

    private static string CreateBulkDataResponse(string type, string downloadUri) {
        var response = new ScryfallBulkDataResponse {
            Data = new List<ScryfallBulkDataItem> {
                new() { Type = type, DownloadUri = downloadUri }
            }
        };
        return JsonSerializer.Serialize(response, new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
    }

    private static ScryfallCard CreateScryfallCard(string name) {
        return new ScryfallCard {
            Id         = Guid.NewGuid().ToString(),
            Name       = name,
            ManaCost   = "{1}{R}",
            Cmc        = 2,
            TypeLine   = "Instant",
            OracleText = "Test oracle text",
            Rarity     = "common",
            Layout     = "normal",
            Colors     = new List<string> { "R" },
            Keywords   = new List<string>()
        };
    }

    #endregion
}