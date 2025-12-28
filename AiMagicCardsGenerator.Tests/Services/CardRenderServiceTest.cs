using System.Collections.Generic;
using System.Text.RegularExpressions;
using AiMagicCardsGenerator.Models.Entities;
using AiMagicCardsGenerator.Services;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AiMagicCardsGenerator.Tests.Services;

[TestClass]
[TestSubject(typeof(CardRenderService))]
public class CardRenderServiceTest {
    #region Single Colors

    [TestMethod]
    public void GetCardColor_WhiteManaCost_ReturnsW() {
        // Arrange
        var card = CreateCard(manaCost: "{1}{W}", typeLine: "Creature");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("W", result);
    }

    [TestMethod]
    public void GetCardColor_BlueManaCost_ReturnsU() {
        // Arrange
        var card = CreateCard(manaCost: "{2}{U}{U}", typeLine: "Instant");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("U", result);
    }

    [TestMethod]
    public void GetCardColor_BlackManaCost_ReturnsB() {
        // Arrange
        var card = CreateCard(manaCost: "{B}{B}{B}", typeLine: "Sorcery");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("B", result);
    }

    [TestMethod]
    public void GetCardColor_RedManaCost_ReturnsR() {
        // Arrange
        var card = CreateCard(manaCost: "{R}", typeLine: "Creature");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("R", result);
    }

    [TestMethod]
    public void GetCardColor_GreenManaCost_ReturnsG() {
        // Arrange
        var card = CreateCard(manaCost: "{3}{G}{G}", typeLine: "Creature");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("G", result);
    }

    #endregion

    #region Multicolor

    [TestMethod]
    public void GetCardColor_TwoColors_ReturnsM() {
        // Arrange
        var card = CreateCard(manaCost: "{W}{U}", typeLine: "Creature");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("M", result);
    }

    [TestMethod]
    public void GetCardColor_ThreeColors_ReturnsM() {
        // Arrange
        var card = CreateCard(manaCost: "{B}{R}{G}", typeLine: "Enchantment");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("M", result);
    }

    [TestMethod]
    public void GetCardColor_FiveColors_ReturnsM() {
        // Arrange
        var card = CreateCard(manaCost: "{W}{U}{B}{R}{G}", typeLine: "Creature");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("M", result);
    }

    #endregion

    #region Colorless

    [TestMethod]
    public void GetCardColor_LandType_ReturnsC() {
        // Arrange
        var card = CreateCard(manaCost: "", typeLine: "Land");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("C", result);
    }

    [TestMethod]
    public void GetCardColor_BasicLand_ReturnsC() {
        // Arrange
        var card = CreateCard(manaCost: "", typeLine: "Basic Land - Forest");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("C", result);
    }

    [TestMethod]
    public void GetCardColor_ColorlessArtifact_ReturnsC() {
        // Arrange
        var card = CreateCard(manaCost: "{1}", typeLine: "Artifact");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("C", result);
    }

    [TestMethod]
    public void GetCardColor_ColorlessCreature_ReturnsC() {
        // Arrange
        var card = CreateCard(manaCost: "{10}", typeLine: "Creature - Eldrazi");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("C", result);
    }

    [TestMethod]
    public void GetCardColor_NullManaCost_ReturnsC() {
        // Arrange
        var card = CreateCard(manaCost: null, typeLine: "Creature");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("C", result);
    }

    [TestMethod]
    public void GetCardColor_EmptyManaCost_ReturnsC() {
        // Arrange
        var card = CreateCard(manaCost: "", typeLine: "Creature");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("C", result);
    }

    [TestMethod]
    public void GetCardColor_GenericManaOnly_ReturnsC() {
        // Arrange
        var card = CreateCard(manaCost: "{5}", typeLine: "Creature");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("C", result);
    }

    #endregion

    #region Colors Property

    [TestMethod]
    public void GetCardColor_ColorsPropertySet_UsesColorsProperty() {
        // Arrange
        var card = CreateCard(manaCost: "{1}", typeLine: "Creature", colors: "R");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("R", result);
    }

    [TestMethod]
    public void GetCardColor_ColorsPropertyJsonArray_ParsesCorrectly() {
        // Arrange
        var card = CreateCard(manaCost: "{1}", typeLine: "Creature", colors: "[\"W\", \"U\"]");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("M", result);
    }

    [TestMethod]
    public void GetCardColor_ColorsPropertySingleInArray_ReturnsSingleColor() {
        // Arrange
        var card = CreateCard(manaCost: "{1}", typeLine: "Creature", colors: "[\"G\"]");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("G", result);
    }

    [TestMethod]
    public void GetCardColor_ColorsPropertyAndManaCost_CombinesBoth() {
        // Arrange
        var card = CreateCard(manaCost: "{R}", typeLine: "Creature", colors: "W");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("M", result);
    }

    #endregion

    #region Artifact Creatures

    [TestMethod]
    public void GetCardColor_ColoredArtifactCreature_ReturnsColor() {
        // Arrange
        var card = CreateCard(manaCost: "{W}", typeLine: "Artifact Creature - Human Soldier");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("W", result);
    }

    [TestMethod]
    public void GetCardColor_MulticolorArtifact_ReturnsM() {
        // Arrange
        var card = CreateCard(manaCost: "{U}{R}", typeLine: "Artifact");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("M", result);
    }

    #endregion

    #region Edge Cases

    [TestMethod]
    public void GetCardColor_LandWithColors_ReturnsC() {
        // Arrange
        var card = CreateCard(manaCost: "", typeLine: "Land - Forest Island", colors: "[\"G\", \"U\"]");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("C", result);
    }

    [TestMethod]
    public void GetCardColor_DuplicateColorsInManaCost_CountsOnce() {
        // Arrange
        var card = CreateCard(manaCost: "{R}{R}{R}{R}", typeLine: "Creature");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("R", result);
    }

    [TestMethod]
    public void GetCardColor_ColorsPropertyWithWhitespace_TrimsCorrectly() {
        // Arrange
        var card = CreateCard(manaCost: "{1}", typeLine: "Creature", colors: "  B  ");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("B", result);
    }

    [TestMethod]
    public void GetCardColor_MixedGenericAndColoredMana_DetectsColor() {
        // Arrange
        var card = CreateCard(manaCost: "{4}{U}{U}", typeLine: "Creature");

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual("U", result);
    }

    #endregion

    #region Data-Driven Tests

    [DataTestMethod]
    [DataRow("{W}",             "Creature", "", "W")]
    [DataRow("{U}",             "Creature", "", "U")]
    [DataRow("{B}",             "Creature", "", "B")]
    [DataRow("{R}",             "Creature", "", "R")]
    [DataRow("{G}",             "Creature", "", "G")]
    [DataRow("{W}{U}",          "Creature", "", "M")]
    [DataRow("{1}",             "Artifact", "", "C")]
    [DataRow("",                "Land",     "", "C")]
    [DataRow("{2}{G}{G}",       "Creature", "", "G")]
    [DataRow("{W}{U}{B}{R}{G}", "Creature", "", "M")]
    public void GetCardColor_VariousInputs_ReturnsExpectedColor(
        string manaCost, string typeLine, string colors, string expected) {
        // Arrange
        var card = CreateCard(manaCost: manaCost, typeLine: typeLine, colors: colors);

        // Act
        var result = CardColorResolver.GetColor(card);

        // Assert
        Assert.AreEqual(expected, result);
    }

    #endregion

    private static Card CreateCard(
        string? manaCost = "",
        string  typeLine = "Creature",
        string  colors   = "",
        string  name     = "Test Card") {
        return new Card {
            Name       = name,
            ManaCost   = manaCost,
            TypeLine   = typeLine,
            Colors     = colors,
            OracleText = ""
        };
    }
}

public static class CardColorResolver {
    public static string GetColor(Card card) {
        var colors = new List<string>();

        if (!string.IsNullOrEmpty(card.Colors)) {
            var colorsStr = card.Colors.Trim();
            if (colorsStr.StartsWith("[")) {
                var matches = Regex.Matches(colorsStr, @"""(\w)""");
                foreach (Match m in matches) {
                    colors.Add(m.Groups[1].Value);
                }
            }
            else {
                colors.Add(colorsStr);
            }
        }

        if (!string.IsNullOrEmpty(card.ManaCost)) {
            if (card.ManaCost.Contains("{W}") && !colors.Contains("W")) colors.Add("W");
            if (card.ManaCost.Contains("{U}") && !colors.Contains("U")) colors.Add("U");
            if (card.ManaCost.Contains("{B}") && !colors.Contains("B")) colors.Add("B");
            if (card.ManaCost.Contains("{R}") && !colors.Contains("R")) colors.Add("R");
            if (card.ManaCost.Contains("{G}") && !colors.Contains("G")) colors.Add("G");
        }

        if (card.TypeLine.Contains("Land"))
            return "C";

        if (card.TypeLine.Contains("Artifact") && colors.Count == 0)
            return "C";

        if (colors.Count > 1)
            return "M";

        if (colors.Count == 1)
            return colors[0];

        return "C";
    }
}