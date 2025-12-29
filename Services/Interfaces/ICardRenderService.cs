using AiMagicCardsGenerator.Models.Entities;

namespace AiMagicCardsGenerator.Services;

public interface ICardRenderService {
    Task<byte[]> RenderCardAsync(Card card);
    string       GetCardColor(Card    card);
}
