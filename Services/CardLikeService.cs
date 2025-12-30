using AiMagicCardsGenerator.Models.Entities;
using AiMagicCardsGenerator.Repositories;

namespace AiMagicCardsGenerator.Services;

public class CardLikeService : ICardLikeService {
    private readonly ICardLikeRepository      _likeRepo;
    private readonly IGeneratedCardRepository _cardRepo;
    private readonly ILikesBroadcastService   _broadcast;

    public CardLikeService(
        ICardLikeRepository      likeRepo,
        IGeneratedCardRepository cardRepo,
        ILikesBroadcastService   broadcast) {
        _likeRepo  = likeRepo;
        _cardRepo  = cardRepo;
        _broadcast = broadcast;
    }

    public async Task<LikeResult> ToggleAsync(int cardId, string userId) {
        var card = await _cardRepo.GetByIdAsync(cardId);
        if (card == null)
            return new LikeResult(false, 0, false);

        var  existing = await _likeRepo.GetAsync(cardId, userId);
        bool isLiked;

        if (existing != null) {
            await _likeRepo.RemoveAsync(existing);
            card.Likes = Math.Max(0, card.Likes - 1);
            isLiked    = false;
        }
        else {
            await _likeRepo.AddAsync(new CardLike { GeneratedCardId = cardId, UserId = userId });
            card.Likes++;
            isLiked = true;
        }

        await _cardRepo.UpdateAsync(card);
        await _broadcast.BroadcastAsync(cardId, card.Likes);

        return new LikeResult(true, card.Likes, isLiked);
    }

    public Task<HashSet<int>> GetUserLikesAsync(string userId, IEnumerable<int> cardIds) =>
        _likeRepo.GetUserLikedCardIdsAsync(userId, cardIds);
}