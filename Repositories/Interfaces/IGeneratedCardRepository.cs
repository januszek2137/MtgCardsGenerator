using AiMagicCardsGenerator.Models.Entities;

namespace AiMagicCardsGenerator.Repositories;

public interface IGeneratedCardRepository {
    Task<GeneratedCard?>      GetByIdAsync(int        id);
    Task<List<GeneratedCard>> GetSharedAsync(int      count = 20);
    Task<List<GeneratedCard>> GetTopLikedAsync(int    count = 6);
    Task<GeneratedCard>       AddAsync(GeneratedCard  card);
    Task                      ShareAsync(int          id);
    Task                      IncrementLikesAsync(int id);
    Task                      SaveChangesAsync();
}