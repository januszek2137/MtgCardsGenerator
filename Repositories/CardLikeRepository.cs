using AiMagicCardsGenerator.Data;
using AiMagicCardsGenerator.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiMagicCardsGenerator.Repositories;

public class CardLikeRepository : ICardLikeRepository {
    private readonly ApplicationDbContext _context;

    public CardLikeRepository(ApplicationDbContext context) {
        _context = context;
    }

    public Task<CardLike?> GetAsync(int cardId, string userId) =>
        _context.CardLikes.FirstOrDefaultAsync(l => l.GeneratedCardId == cardId && l.UserId == userId);

    public async Task AddAsync(CardLike like) {
        await _context.CardLikes.AddAsync(like);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(CardLike like) {
        _context.CardLikes.Remove(like);
        await _context.SaveChangesAsync();
    }

    public async Task<HashSet<int>> GetUserLikedCardIdsAsync(string userId, IEnumerable<int> cardIds) {
        var ids = await _context.CardLikes
            .Where(l => l.UserId == userId && cardIds.Contains(l.GeneratedCardId))
            .Select(l => l.GeneratedCardId)
            .ToListAsync();
        return ids.ToHashSet();
    }
}