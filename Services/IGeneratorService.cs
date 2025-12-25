using AiMagicCardsGenerator.Models.Entities;

namespace AiMagicCardsGenerator.Services;

public interface IGeneratorService {
    Task<CardGenerationResult> GenerateRandomCardAsync();
}

public class CardGenerationResult
{
    public Card       Card    { get; set; } = null!;
    public List<Card> BasedOn { get; set; } = [];
}