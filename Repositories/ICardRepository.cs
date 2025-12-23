using AiMagicCardsGenerator.Models.Entities;

namespace AiMagicCardsGenerator.Repositories;

public interface ICardRepository
{
    Task<Card?> GetByIdAsync(int id);
    Task<List<Card>> GetAllAsync(int skip = 0, int take = 100);
    Task<List<Card>> SearchAsync(string? type = null, string? colors = null, decimal? minCmc = null, decimal? maxCmc = null, int take = 10);
    Task<List<Card>> GetRandomAsync(int count = 5);
    Task<List<Card>> GetRandomByCmcAsync(int cmc, int count = 5);
    Task<int> GetCountAsync();
    Task AddRangeAsync(IEnumerable<Card> cards);
    Task<bool> AnyAsync();
    Task SaveChangesAsync();
}