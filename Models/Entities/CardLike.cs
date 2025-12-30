namespace AiMagicCardsGenerator.Models.Entities;

public class CardLike {
    public int      Id              { get; set; }
    public int      GeneratedCardId { get; set; }
    public string   UserId          { get; set; } = "";
    public DateTime CreatedAt       { get; set; } = DateTime.UtcNow;

    public GeneratedCard GeneratedCard { get; set; } = null!;
}