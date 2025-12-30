using AiMagicCardsGenerator.Models.Entities;

namespace AiMagicCardsGenerator.Repositories;

public interface IGeneratedCardRepository {
    Task<GeneratedCard?>      GetByIdAsync(int          id);
    Task<List<GeneratedCard>> GetSharedAsync(int        count = 20);
    Task<List<GeneratedCard>> GetTopLikedAsync(int      count = 6);
    Task<List<GeneratedCard>> GetByUserIdAsync(string   userId);
    Task<bool>                IsOwnerAsync(int          cardId, string userId);
    Task<GeneratedCard>       AddAsync(GeneratedCard    card);
    Task                      UpdateAsync(GeneratedCard card);
    Task                      ShareAsync(int          id);
    Task                      IncrementLikesAsync(int id);
    Task                      SaveChangesAsync();
}