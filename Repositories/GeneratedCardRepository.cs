using AiMagicCardsGenerator.Data;
using AiMagicCardsGenerator.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiMagicCardsGenerator.Repositories;

public class GeneratedCardRepository : IGeneratedCardRepository {
    private readonly ApplicationDbContext _context;

    public GeneratedCardRepository(ApplicationDbContext context) {
        _context = context;
    }

    public async Task<GeneratedCard?> GetByIdAsync(int id) {
        return await _context.GeneratedCards.FindAsync(id);
    }

    public async Task<List<GeneratedCard>> GetSharedAsync(int count = 20) {
        return await _context.GeneratedCards
            .Where(c => c.IsShared)
            .OrderByDescending(c => c.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<GeneratedCard>> GetTopLikedAsync(int count = 6) {
        return await _context.GeneratedCards
            .Where(c => c.IsShared)
            .OrderByDescending(c => c.Likes)
            .ThenByDescending(c => c.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<GeneratedCard>> GetByUserIdAsync(string userId) {
        return await _context.GeneratedCards
            .Where(c => c.CreatedBy == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> IsOwnerAsync(int cardId, string userId) {
        return await _context.GeneratedCards
            .AnyAsync(c => c.Id == cardId && c.CreatedBy == userId);
    }

    public async Task<GeneratedCard> AddAsync(GeneratedCard card) {
        _context.GeneratedCards.Add(card);
        await _context.SaveChangesAsync();
        return card;
    }

    public async Task UpdateAsync(GeneratedCard card) {
        _context.GeneratedCards.Update(card);
        await _context.SaveChangesAsync();
    }

    public async Task ShareAsync(int id) {
        await _context.GeneratedCards
            .Where(c => c.Id == id)
            .ExecuteUpdateAsync(c => c.SetProperty(x => x.IsShared, true));
    }

    public async Task IncrementLikesAsync(int id) {
        await _context.GeneratedCards
            .Where(c => c.Id == id)
            .ExecuteUpdateAsync(c => c.SetProperty(x => x.Likes, x => x.Likes + 1));
    }

    public async Task SaveChangesAsync() {
        await _context.SaveChangesAsync();
    }
}