namespace AiMagicCardsGenerator.Models.Entities;

public class Card
{
    public int Id { get; set; }
    public string ScryfallId { get; set; } = "";
    public string Name { get; set; } = "";
    public string? ManaCost { get; set; }
    public decimal Cmc { get; set; }
    public string TypeLine { get; set; } = "";
    public string? OracleText { get; set; }
    public string? Power { get; set; }
    public string? Toughness { get; set; }
    public string Colors { get; set; } = "";
    public string Keywords { get; set; } = "";
    public string Rarity { get; set; } = "";
    public string? FlavorText { get; set; }
}