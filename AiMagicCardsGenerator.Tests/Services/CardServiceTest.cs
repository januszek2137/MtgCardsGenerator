using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AiMagicCardsGenerator.Models.Dto;
using AiMagicCardsGenerator.Models.Entities;
using AiMagicCardsGenerator.Repositories;
using AiMagicCardsGenerator.Services;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using AiMagicCardsGenerator.Models.Entities;
using AiMagicCardsGenerator.Models.Dto;
using AiMagicCardsGenerator.Repositories;
using AiMagicCardsGenerator.Services;
using Moq;

namespace AiMagicCardsGenerator.Tests.Services;

[TestClass]
[TestSubject(typeof(CardService))]
public class CardServiceTest {

    private Mock<ICardRepository>  _mockRepository = null!;
    private Mock<IScryfallService> _mockScryfall   = null!;
    private CardService            _service        = null!;

    [TestInitialize]
    public void Setup() {
        _mockRepository = new Mock<ICardRepository>();
        _mockScryfall   = new Mock<IScryfallService>();
        _service        = new CardService(_mockRepository.Object, _mockScryfall.Object);
    }

    #region ImportCardsFromScryfallAsync

    [TestMethod]
    public async Task ImportCardsFromScryfallAsync_ValidCards_ReturnsCount() {
        // Arrange
        var scryfallCards = new List<ScryfallCard> {
            CreateScryfallCard(layout: "normal", oracleText: "Draw a card"),
            CreateScryfallCard(layout: "normal", oracleText: "Deal 3 damage")
        };
        _mockScryfall.Setup(s => s.DownloadAllCardsAsync()).ReturnsAsync(scryfallCards);

        // Act
        var result = await _service.ImportCardsFromScryfallAsync();

        // Assert
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public async Task ImportCardsFromScryfallAsync_FiltersTokens_ExcludesFromCount() {
        // Arrange
        var scryfallCards = new List<ScryfallCard> {
            CreateScryfallCard(layout: "normal", oracleText: "Draw a card"),
            CreateScryfallCard(layout: "token",  oracleText: "Token creature"),
            CreateScryfallCard(layout: "emblem", oracleText: "Emblem text")
        };
        _mockScryfall.Setup(s => s.DownloadAllCardsAsync()).ReturnsAsync(scryfallCards);

        // Act
        var result = await _service.ImportCardsFromScryfallAsync();

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task ImportCardsFromScryfallAsync_FiltersArtSeries_ExcludesFromCount() {
        // Arrange
        var scryfallCards = new List<ScryfallCard> {
            CreateScryfallCard(layout: "normal",     oracleText: "Valid card"),
            CreateScryfallCard(layout: "art_series", oracleText: "Art card")
        };
        _mockScryfall.Setup(s => s.DownloadAllCardsAsync()).ReturnsAsync(scryfallCards);

        // Act
        var result = await _service.ImportCardsFromScryfallAsync();

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task ImportCardsFromScryfallAsync_EmptyOracleText_ExcludesCard() {
        // Arrange
        var scryfallCards = new List<ScryfallCard> {
            CreateScryfallCard(layout: "normal", oracleText: "Valid text"),
            CreateScryfallCard(layout: "normal", oracleText: ""),
            CreateScryfallCard(layout: "normal", oracleText: null)
        };
        _mockScryfall.Setup(s => s.DownloadAllCardsAsync()).ReturnsAsync(scryfallCards);

        // Act
        var result = await _service.ImportCardsFromScryfallAsync();

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task ImportCardsFromScryfallAsync_LandWithoutOracleText_IncludesCard() {
        // Arrange
        var scryfallCards = new List<ScryfallCard> {
            CreateScryfallCard(layout: "normal", oracleText: "", typeLine: "Basic Land - Forest")
        };
        _mockScryfall.Setup(s => s.DownloadAllCardsAsync()).ReturnsAsync(scryfallCards);

        // Act
        var result = await _service.ImportCardsFromScryfallAsync();

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task ImportCardsFromScryfallAsync_CallsRepositoryAddRange() {
        // Arrange
        var scryfallCards = new List<ScryfallCard> {
            CreateScryfallCard(layout: "normal", oracleText: "Test")
        };
        _mockScryfall.Setup(s => s.DownloadAllCardsAsync()).ReturnsAsync(scryfallCards);

        // Act
        await _service.ImportCardsFromScryfallAsync();

        // Assert
        _mockRepository.Verify(r => r.AddRangeAsync(It.IsAny<List<Card>>()), Times.Once);
    }

    [TestMethod]
    public async Task ImportCardsFromScryfallAsync_CallsSaveChanges() {
        // Arrange
        var scryfallCards = new List<ScryfallCard> {
            CreateScryfallCard(layout: "normal", oracleText: "Test")
        };
        _mockScryfall.Setup(s => s.DownloadAllCardsAsync()).ReturnsAsync(scryfallCards);

        // Act
        await _service.ImportCardsFromScryfallAsync();

        // Assert
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task ImportCardsFromScryfallAsync_MapsFieldsCorrectly() {
        // Arrange
        var scryfallCard = new ScryfallCard {
            Id         = Guid.NewGuid().ToString(),
            Name       = "Lightning Bolt",
            ManaCost   = "{R}",
            Cmc        = 1,
            TypeLine   = "Instant",
            OracleText = "Deal 3 damage",
            Power      = null,
            Toughness  = null,
            Rarity     = "common",
            FlavorText = "Zap!",
            Colors     = new List<string> { "R" },
            Keywords   = new List<string> { },
            Layout     = "normal"
        };
        _mockScryfall.Setup(s => s.DownloadAllCardsAsync()).ReturnsAsync(new List<ScryfallCard> { scryfallCard });

        List<Card>? capturedCards = null;
        _mockRepository
            .Setup(r => r.AddRangeAsync(It.IsAny<List<Card>>()))
            .Callback<IEnumerable<Card>>(cards => capturedCards = cards.ToList());
        // Act
        await _service.ImportCardsFromScryfallAsync();

        // Assert
        Assert.IsNotNull(capturedCards);
        Assert.HasCount(1, capturedCards);

        var card = capturedCards[0];
        Assert.AreEqual(scryfallCard.Id,         card.ScryfallId);
        Assert.AreEqual(scryfallCard.Name,       card.Name);
        Assert.AreEqual(scryfallCard.ManaCost,   card.ManaCost);
        Assert.AreEqual(scryfallCard.Cmc,        card.Cmc);
        Assert.AreEqual(scryfallCard.TypeLine,   card.TypeLine);
        Assert.AreEqual(scryfallCard.OracleText, card.OracleText);
        Assert.AreEqual(scryfallCard.Rarity,     card.Rarity);
        Assert.AreEqual(scryfallCard.FlavorText, card.FlavorText);
    }

    [TestMethod]
    public async Task ImportCardsFromScryfallAsync_SerializesColorsToJson() {
        // Arrange
        var scryfallCard = CreateScryfallCard(
            layout: "normal",
            oracleText: "Test",
            colors: new List<string> { "W", "U" }
        );
        _mockScryfall.Setup(s => s.DownloadAllCardsAsync()).ReturnsAsync(new List<ScryfallCard> { scryfallCard });

        List<Card>? capturedCards = null;
        _mockRepository
            .Setup(r => r.AddRangeAsync(It.IsAny<List<Card>>()))
            .Callback<IEnumerable<Card>>(cards => capturedCards = cards.ToList());

        // Act
        await _service.ImportCardsFromScryfallAsync();

        // Assert
        Assert.IsNotNull(capturedCards);
        Assert.AreEqual("[\"W\",\"U\"]", capturedCards[0].Colors);
    }

    [TestMethod]
    public async Task ImportCardsFromScryfallAsync_SerializesKeywordsToJson() {
        // Arrange
        var scryfallCard = CreateScryfallCard(
            layout: "normal",
            oracleText: "Test",
            keywords: new List<string> { "Flying", "Haste" }
        );
        _mockScryfall.Setup(s => s.DownloadAllCardsAsync()).ReturnsAsync(new List<ScryfallCard> { scryfallCard });

        List<Card>? capturedCards = null;
        _mockRepository
            .Setup(r => r.AddRangeAsync(It.IsAny<List<Card>>()))
            .Callback<IEnumerable<Card>>(cards => capturedCards = cards.ToList());

        // Act
        await _service.ImportCardsFromScryfallAsync();

        // Assert
        Assert.IsNotNull(capturedCards);
        Assert.AreEqual("[\"Flying\",\"Haste\"]", capturedCards[0].Keywords);
    }

    [TestMethod]
    public async Task ImportCardsFromScryfallAsync_EmptyList_ReturnsZero() {
        // Arrange
        _mockScryfall.Setup(s => s.DownloadAllCardsAsync()).ReturnsAsync(new List<ScryfallCard>());

        // Act
        var result = await _service.ImportCardsFromScryfallAsync();

        // Assert
        Assert.AreEqual(0, result);
    }

    #endregion

    #region GetCardsAsync

    [TestMethod]
    public async Task GetCardsAsync_DefaultParameters_UsesCorrectSkipAndTake() {
        // Arrange
        _mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<Card>());

        // Act
        await _service.GetCardsAsync();

        // Assert
        _mockRepository.Verify(r => r.GetAllAsync(0, 100), Times.Once);
    }

    [TestMethod]
    public async Task GetCardsAsync_Page2_CalculatesSkipCorrectly() {
        // Arrange
        _mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<Card>());

        // Act
        await _service.GetCardsAsync(page: 2, pageSize: 50);

        // Assert
        _mockRepository.Verify(r => r.GetAllAsync(50, 50), Times.Once);
    }

    [TestMethod]
    public async Task GetCardsAsync_Page3_CalculatesSkipCorrectly() {
        // Arrange
        _mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<Card>());

        // Act
        await _service.GetCardsAsync(page: 3, pageSize: 25);

        // Assert
        _mockRepository.Verify(r => r.GetAllAsync(50, 25), Times.Once);
    }

    [TestMethod]
    public async Task GetCardsAsync_ReturnsCardsFromRepository() {
        // Arrange
        var expectedCards = new List<Card> {
            new() { Id = 1, Name = "Card 1" },
            new() { Id = 2, Name = "Card 2" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(expectedCards);

        // Act
        var result = await _service.GetCardsAsync();

        // Assert
        Assert.HasCount(2, result);
        Assert.AreEqual("Card 1", result[0].Name);
        Assert.AreEqual("Card 2", result[1].Name);
    }

    #endregion

    #region SearchCardsAsync

    [TestMethod]
    public async Task SearchCardsAsync_PassesParametersToRepository() {
        // Arrange
        _mockRepository
            .Setup(r => r.SearchAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<decimal?>(),
                It.IsAny<decimal?>(), It.IsAny<int>()))
            .ReturnsAsync(new List<Card>());

        // Act
        await _service.SearchCardsAsync(type: "Creature", colors: "R", minCmc: 2, maxCmc: 5, take: 20);

        // Assert
        _mockRepository.Verify(r => r.SearchAsync("Creature", "R", 2, 5, 20), Times.Once);
    }

    [TestMethod]
    public async Task SearchCardsAsync_NullParameters_PassesNulls() {
        // Arrange
        _mockRepository
            .Setup(r => r.SearchAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<decimal?>(),
                It.IsAny<decimal?>(), It.IsAny<int>()))
            .ReturnsAsync(new List<Card>());

        // Act
        await _service.SearchCardsAsync(null, null, null, null);

        // Assert
        _mockRepository.Verify(r => r.SearchAsync(null, null, null, null, 10), Times.Once);
    }

    [TestMethod]
    public async Task SearchCardsAsync_ReturnsMatchingCards() {
        // Arrange
        var expectedCards = new List<Card> {
            new() { Id = 1, Name = "Goblin", TypeLine = "Creature - Goblin" }
        };
        _mockRepository
            .Setup(r => r.SearchAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<decimal?>(),
                It.IsAny<decimal?>(), It.IsAny<int>()))
            .ReturnsAsync(expectedCards);

        // Act
        var result = await _service.SearchCardsAsync(type: "Creature", colors: null, minCmc: null, maxCmc: null);

        // Assert
        Assert.HasCount(1, result);
        Assert.AreEqual("Goblin", result[0].Name);
    }

    #endregion

    #region GetCardAsync

    [TestMethod]
    public async Task GetCardAsync_ExistingId_ReturnsCard() {
        // Arrange
        var expectedCard = new Card { Id = 42, Name = "Test Card" };
        _mockRepository.Setup(r => r.GetByIdAsync(42)).ReturnsAsync(expectedCard);

        // Act
        var result = await _service.GetCardAsync(42);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(42,          result.Id);
        Assert.AreEqual("Test Card", result.Name);
    }

    [TestMethod]
    public async Task GetCardAsync_NonExistingId_ReturnsNull() {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Card?)null);

        // Act
        var result = await _service.GetCardAsync(999);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetCardAsync_CallsRepositoryWithCorrectId() {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Card?)null);

        // Act
        await _service.GetCardAsync(123);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(123), Times.Once);
    }

    #endregion

    #region GetTotalCountAsync

    [TestMethod]
    public async Task GetTotalCountAsync_ReturnsRepositoryCount() {
        // Arrange
        _mockRepository.Setup(r => r.GetCountAsync()).ReturnsAsync(1500);

        // Act
        var result = await _service.GetTotalCountAsync();

        // Assert
        Assert.AreEqual(1500, result);
    }

    [TestMethod]
    public async Task GetTotalCountAsync_EmptyRepository_ReturnsZero() {
        // Arrange
        _mockRepository.Setup(r => r.GetCountAsync()).ReturnsAsync(0);

        // Act
        var result = await _service.GetTotalCountAsync();

        // Assert
        Assert.AreEqual(0, result);
    }

    #endregion

    #region HasDataAsync

    [TestMethod]
    public async Task HasDataAsync_DataExists_ReturnsTrue() {
        // Arrange
        _mockRepository.Setup(r => r.AnyAsync()).ReturnsAsync(true);

        // Act
        var result = await _service.HasDataAsync();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task HasDataAsync_NoData_ReturnsFalse() {
        // Arrange
        _mockRepository.Setup(r => r.AnyAsync()).ReturnsAsync(false);

        // Act
        var result = await _service.HasDataAsync();

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    private static ScryfallCard CreateScryfallCard(
        string        layout     = "normal",
        string?       oracleText = "Test oracle text",
        string        typeLine   = "Creature - Human",
        List<string>? colors     = null,
        List<string>? keywords   = null) {
        return new ScryfallCard {
            Id         = Guid.NewGuid().ToString(),
            Name       = "Test Card",
            ManaCost   = "{1}{R}",
            Cmc        = 2,
            TypeLine   = typeLine,
            OracleText = oracleText,
            Power      = "2",
            Toughness  = "2",
            Rarity     = "common",
            FlavorText = "Flavor",
            Colors     = colors   ?? new List<string> { "R" },
            Keywords   = keywords ?? new List<string>(),
            Layout     = layout
        };
    }
}