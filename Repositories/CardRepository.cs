using AiMagicCardsGenerator.Data;
using AiMagicCardsGenerator.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiMagicCardsGenerator.Repositories;

public class CardRepository : ICardRepository {
    private readonly ApplicationDbContext _context;

    public CardRepository(ApplicationDbContext context) {
        _context = context;
    }

    public async Task<Card?> GetByIdAsync(int id) {
        return await _context.Cards.FindAsync(id);
    }

    public async Task<List<Card>> GetAllAsync(int skip = 0, int take = 100) {
        return await _context.Cards
                             .OrderBy(c => c.Name)
                             .Skip(skip)
                             .Take(take)
                             .ToListAsync();
    }

    public async Task<List<Card>> SearchAsync(string?  type   = null, string? colors = null, decimal? minCmc = null,
                                              decimal? maxCmc = null, int     take   = 10) {
        var query = _context.Cards.AsQueryable();

        if (!string.IsNullOrEmpty(type))
            query = query.Where(c => c.TypeLine.Contains(type));

        if (!string.IsNullOrEmpty(colors))
            query = query.Where(c => c.Colors.Contains(colors));

        if (minCmc.HasValue)
            query = query.Where(c => c.Cmc >= minCmc.Value);

        if (maxCmc.HasValue)
            query = query.Where(c => c.Cmc <= maxCmc.Value);

        return await query.Take(take).ToListAsync();
    }

    public async Task<List<Card>> GetRandomAsync(int count = 5) {
        return await _context.Cards
            .OrderBy(c => EF.Functions.Random())
            .Take(count)
            .ToListAsync();
    }
    
    public async Task<List<Card>> GetRandomByCmcAsync(int cmc, int count = 5)
    {
        return await _context.Cards
            .Where(c => c.Cmc == cmc)
            .OrderBy(c => EF.Functions.Random())
            .Take(count)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync() {
        return await _context.Cards.CountAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Card> cards) {
        await _context.Cards.AddRangeAsync(cards);
    }

    public async Task<bool> AnyAsync() {
        return await _context.Cards.AnyAsync();
    }

    public async Task SaveChangesAsync() {
        await _context.SaveChangesAsync();
    }
}