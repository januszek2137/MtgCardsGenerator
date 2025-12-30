namespace AiMagicCardsGenerator.Services;

public record LikeResult(bool Success, int Likes, bool IsLiked);

public interface ICardLikeService {
    Task<LikeResult>   ToggleAsync(int          cardId, string           userId);
    Task<HashSet<int>> GetUserLikesAsync(string userId, IEnumerable<int> cardIds);
}