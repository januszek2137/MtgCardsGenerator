using AiMagicCardsGenerator.Models.Entities;

namespace AiMagicCardsGenerator.Repositories;

public interface ICardLikeRepository {
    Task<CardLike?>    GetAsync(int                    cardId, string userId);
    Task               AddAsync(CardLike               like);
    Task               RemoveAsync(CardLike            like);
    Task<HashSet<int>> GetUserLikedCardIdsAsync(string userId, IEnumerable<int> cardIds);
}