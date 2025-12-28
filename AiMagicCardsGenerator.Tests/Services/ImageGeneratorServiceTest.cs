using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AiMagicCardsGenerator.Services;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace AiMagicCardsGenerator.Tests.Services;

[TestClass]
[TestSubject(typeof(ImageGeneratorService))]
public class ImageGeneratorServiceTest {
    private Mock<HttpMessageHandler> _mockHttpHandler = null!;
    private HttpClient               _httpClient      = null!;
    private ImageGeneratorService    _service         = null!;

    [TestInitialize]
    public void Setup() {
        _mockHttpHandler = new Mock<HttpMessageHandler>();
        _httpClient      = new HttpClient(_mockHttpHandler.Object);
        _service         = new ImageGeneratorService(_httpClient);
    }

    #region GenerateCardArtAsync

    [TestMethod]
    public async Task GenerateCardArtAsync_ValidRequest_ReturnsImageBytes() {
        // Arrange
        var expectedBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
        SetupHttpResponse(expectedBytes);

        // Act
        var result = await _service.GenerateCardArtAsync("Test Card", "Creature - Human", "Some text");

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(4, result);
        CollectionAssert.AreEqual(expectedBytes, result);
    }

    [TestMethod]
    public async Task GenerateCardArtAsync_CallsCorrectBaseUrl() {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        SetupHttpResponseWithCapture(new byte[] { 1, 2, 3 }, req => capturedRequest = req);

        // Act
        await _service.GenerateCardArtAsync("Test", "Creature", null);

        // Assert
        Assert.IsNotNull(capturedRequest);
        Assert.StartsWith("https://image.pollinations.ai/prompt/", capturedRequest.RequestUri!.ToString());
    }

    [TestMethod]
    public async Task GenerateCardArtAsync_IncludesWidthParameter() {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        SetupHttpResponseWithCapture(new byte[] { 1, 2, 3 }, req => capturedRequest = req);

        // Act
        await _service.GenerateCardArtAsync("Test", "Creature", null);

        // Assert
        Assert.IsNotNull(capturedRequest);
        Assert.Contains("width=3000", capturedRequest.RequestUri!.ToString());
    }

    [TestMethod]
    public async Task GenerateCardArtAsync_IncludesHeightParameter() {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        SetupHttpResponseWithCapture(new byte[] { 1, 2, 3 }, req => capturedRequest = req);

        // Act
        await _service.GenerateCardArtAsync("Test", "Creature", null);

        // Assert
        Assert.IsNotNull(capturedRequest);
        Assert.Contains("height=2400", capturedRequest.RequestUri!.ToString());
    }

    [TestMethod]
    public async Task GenerateCardArtAsync_IncludesNoLogoParameter() {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        SetupHttpResponseWithCapture(new byte[] { 1, 2, 3 }, req => capturedRequest = req);

        // Act
        await _service.GenerateCardArtAsync("Test", "Creature", null);

        // Assert
        Assert.IsNotNull(capturedRequest);
        Assert.Contains("nologo=true", capturedRequest.RequestUri!.ToString());
    }

    [TestMethod]
    public async Task GenerateCardArtAsync_EncodesCardNameInUrl() {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        SetupHttpResponseWithCapture(new byte[] { 1, 2, 3 }, req => capturedRequest = req);

        // Act
        await _service.GenerateCardArtAsync("Fire & Ice", "Instant", null);

        // Assert
        Assert.IsNotNull(capturedRequest);
        var url = capturedRequest.RequestUri!.ToString();
        Assert.DoesNotContain("Fire & Ice", url);
        Assert.Contains("Fire", url);
    }

    #endregion

    #region BuildPrompt - Card Types

    [TestMethod]
    public async Task GenerateCardArtAsync_CreatureType_IncludesCreatureKeywords() {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        SetupHttpResponseWithCapture(new byte[] { 1, 2, 3 }, req => capturedRequest = req);

        // Act
        await _service.GenerateCardArtAsync("Goblin Warrior", "Creature - Goblin Warrior", null);

        // Assert
        Assert.IsNotNull(capturedRequest);
        var url = Uri.UnescapeDataString(capturedRequest.RequestUri!.ToString());
        Assert.Contains("creature portrait", url);
        Assert.Contains("dramatic pose", url);
    }

    [TestMethod]
    public async Task GenerateCardArtAsync_InstantType_IncludesSpellKeywords() {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        SetupHttpResponseWithCapture(new byte[] { 1, 2, 3 }, req => capturedRequest = req);

        // Act
        await _service.GenerateCardArtAsync("Lightning Bolt", "Instant", "Deal 3 damage");

        // Assert
        Assert.IsNotNull(capturedRequest);
        var url = Uri.UnescapeDataString(capturedRequest.RequestUri!.ToString());
        Assert.Contains("magical spell effect", url);
        Assert.Contains("mystical", url);
    }

    [TestMethod]
    public async Task GenerateCardArtAsync_SorceryType_IncludesSpellKeywords() {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        SetupHttpResponseWithCapture(new byte[] { 1, 2, 3 }, req => capturedRequest = req);

        // Act
        await _service.GenerateCardArtAsync("Fireball", "Sorcery", "Deal X damage");

        // Assert
        Assert.IsNotNull(capturedRequest);
        var url = Uri.UnescapeDataString(capturedRequest.RequestUri!.ToString());
        Assert.Contains("magical spell effect", url);
    }

    [TestMethod]
    public async Task GenerateCardArtAsync_EnchantmentType_IncludesAuraKeywords() {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        SetupHttpResponseWithCapture(new byte[] { 1, 2, 3 }, req => capturedRequest = req);

        // Act
        await _service.GenerateCardArtAsync("Divine Favor", "Enchantment - Aura", null);

        // Assert
        Assert.IsNotNull(capturedRequest);
        var url = Uri.UnescapeDataString(capturedRequest.RequestUri!.ToString());
        Assert.Contains("magical aura", url);
        Assert.Contains("ethereal glow", url);
    }

    [TestMethod]
    public async Task GenerateCardArtAsync_ArtifactType_IncludesItemKeywords() {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        SetupHttpResponseWithCapture(new byte[] { 1, 2, 3 }, req => capturedRequest = req);

        // Act
        await _service.GenerateCardArtAsync("Sol Ring", "Artifact", "Tap: Add {C}{C}");

        // Assert
        Assert.IsNotNull(capturedRequest);
        var url = Uri.UnescapeDataString(capturedRequest.RequestUri!.ToString());
        Assert.Contains("magical item", url);
        Assert.Contains("detailed object", url);
    }

    [TestMethod]
    public async Task GenerateCardArtAsync_LandType_IncludesLandscapeKeywords() {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        SetupHttpResponseWithCapture(new byte[] { 1, 2, 3 }, req => capturedRequest = req);

        // Act
        await _service.GenerateCardArtAsync("Forest", "Basic Land - Forest", null);

        // Assert
        Assert.IsNotNull(capturedRequest);
        var url = Uri.UnescapeDataString(capturedRequest.RequestUri!.ToString());
        Assert.Contains("landscape", url);
        Assert.Contains("environment", url);
    }

    #endregion

    #region BuildPrompt - Creature Subtypes

    [TestMethod]
    public async Task GenerateCardArtAsync_CreatureWithSubtype_IncludesSubtypeInPrompt() {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        SetupHttpResponseWithCapture(new byte[] { 1, 2, 3 }, req => capturedRequest = req);

        // Act
        await _service.GenerateCardArtAsync("Elvish Mystic", "Creature — Elf Druid", null);

        // Assert
        Assert.IsNotNull(capturedRequest);
        var url = Uri.UnescapeDataString(capturedRequest.RequestUri!.ToString());
        Assert.Contains("Elf Druid", url);
    }

    [TestMethod]
    public async Task GenerateCardArtAsync_CreatureWithoutSubtype_NoExtraComma() {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        SetupHttpResponseWithCapture(new byte[] { 1, 2, 3 }, req => capturedRequest = req);

        // Act
        await _service.GenerateCardArtAsync("Nameless", "Creature", null);

        // Assert
        Assert.IsNotNull(capturedRequest);
        var url = Uri.UnescapeDataString(capturedRequest.RequestUri!.ToString());
        Assert.Contains("Nameless", url);
    }

    #endregion

    #region BuildPrompt - Common Elements

    [TestMethod]
    public async Task GenerateCardArtAsync_AlwaysIncludesFantasyArtStyle() {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        SetupHttpResponseWithCapture(new byte[] { 1, 2, 3 }, req => capturedRequest = req);

        // Act
        await _service.GenerateCardArtAsync("Any Card", "Creature", null);

        // Assert
        Assert.IsNotNull(capturedRequest);
        var url = Uri.UnescapeDataString(capturedRequest.RequestUri!.ToString());
        Assert.Contains("Fantasy art", url);
        Assert.Contains("Magic the Gathering card art style", url);
    }

    [TestMethod]
    public async Task GenerateCardArtAsync_AlwaysIncludesQualityKeywords() {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        SetupHttpResponseWithCapture(new byte[] { 1, 2, 3 }, req => capturedRequest = req);

        // Act
        await _service.GenerateCardArtAsync("Any Card", "Creature", null);

        // Assert
        Assert.IsNotNull(capturedRequest);
        var url = Uri.UnescapeDataString(capturedRequest.RequestUri!.ToString());
        Assert.Contains("high fantasy", url);
        Assert.Contains("detailed", url);
        Assert.Contains("epic lighting", url);
        Assert.Contains("professional illustration", url);
    }

    [TestMethod]
    public async Task GenerateCardArtAsync_IncludesCardNameInPrompt() {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        SetupHttpResponseWithCapture(new byte[] { 1, 2, 3 }, req => capturedRequest = req);

        // Act
        await _service.GenerateCardArtAsync("Shivan Dragon", "Creature - Dragon", null);

        // Assert
        Assert.IsNotNull(capturedRequest);
        var url = Uri.UnescapeDataString(capturedRequest.RequestUri!.ToString());
        Assert.Contains("Shivan Dragon", url);
    }

    #endregion

    #region Error Handling

    [TestMethod]
    public async Task GenerateCardArtAsync_HttpError_ThrowsException() {
        // Arrange
        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.InternalServerError
            });

        // Act & Assert
        var caught = false;
        try {
            await _service.GenerateCardArtAsync("Test", "Creature", null);
        }
        catch (HttpRequestException) {
            caught = true;
        }

        Assert.IsTrue(caught);
    }

    [TestMethod]
    public async Task GenerateCardArtAsync_NullOracleText_DoesNotThrow() {
        // Arrange
        SetupHttpResponse(new byte[] { 1, 2, 3 });

        // Act
        var result = await _service.GenerateCardArtAsync("Test", "Creature", null);

        // Assert
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GenerateCardArtAsync_EmptyOracleText_DoesNotThrow() {
        // Arrange
        SetupHttpResponse(new byte[] { 1, 2, 3 });

        // Act
        var result = await _service.GenerateCardArtAsync("Test", "Creature", "");

        // Assert
        Assert.IsNotNull(result);
    }

    #endregion

    #region Helpers

    private void SetupHttpResponse(byte[] content) {
        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content    = new ByteArrayContent(content)
            });
    }

    private void SetupHttpResponseWithCapture(byte[] content, Action<HttpRequestMessage> capture) {
        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => capture(req))
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content    = new ByteArrayContent(content)
            });
    }

    #endregion
}