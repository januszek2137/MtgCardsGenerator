namespace AiMagicCardsGenerator.Models.Entities;

public class GeneratedCard {
    public int      Id         { get; set; }
    public string   Name       { get; set; } = "";
    public string?  ManaCost   { get; set; }
    public decimal  Cmc        { get; set; }
    public string   TypeLine   { get; set; } = "";
    public string?  OracleText { get; set; }
    public string?  Power      { get; set; }
    public string?  Toughness  { get; set; }
    public string   Colors     { get; set; } = "";
    public string?  FlavorText { get; set; }
    public byte[]   ImageData  { get; set; } = [];
    public DateTime CreatedAt  { get; set; } = DateTime.UtcNow;
    public string   CreatorIp  { get; set; } = "";
    public bool     IsShared   { get; set; } = false;
    public int      Likes      { get; set; } = 0;
}