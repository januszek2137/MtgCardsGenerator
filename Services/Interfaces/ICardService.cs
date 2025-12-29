using AiMagicCardsGenerator.Models.Entities;

namespace AiMagicCardsGenerator.Services;

public interface ICardService {
    Task<int>        ImportCardsFromScryfallAsync();
    Task<List<Card>> GetCardsAsync(int        page = 1, int pageSize = 100);
    Task<List<Card>> SearchCardsAsync(string? type, string? colors, decimal? minCmc, decimal? maxCmc, int take = 10);
    Task<Card?>      GetCardAsync(int         id);
    Task<int>        GetTotalCountAsync();
    Task<bool>       HasDataAsync();
}