namespace AiMagicCardsGenerator.Services;

public interface IImageGeneratorService {
    Task<byte[]> GenerateCardArtAsync(string cardName, string typeLine, string? oracleText);
}