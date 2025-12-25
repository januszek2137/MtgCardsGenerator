using System.Text.Json;
using AiMagicCardsGenerator.Models.Entities;
using AiMagicCardsGenerator.Repositories;

namespace AiMagicCardsGenerator.Services;

public class CardService:ICardService {
    private readonly ICardRepository  _cardRepository;
    private readonly IScryfallService _scryfallService;

    public CardService(ICardRepository cardRepository, IScryfallService scryfallService) {
        _cardRepository  = cardRepository;
        _scryfallService = scryfallService;
    }

    public async Task<int> ImportCardsFromScryfallAsync() {
        var scryfallCards = await _scryfallService.DownloadAllCardsAsync();

        var validCards = scryfallCards
                        .Where(c => c.Layout is not ("token" or "emblem" or "art_series"))
                        .Where(c => !string.IsNullOrEmpty(c.OracleText) || c.TypeLine.Contains("Land"))
                        .ToList();

        var entities = validCards.Select(sc => new Card {
                                                            ScryfallId = sc.Id,
                                                            Name       = sc.Name,
                                                            ManaCost   = sc.ManaCost,
                                                            Cmc        = sc.Cmc,
                                                            TypeLine   = sc.TypeLine,
                                                            OracleText = sc.OracleText,
                                                            Power      = sc.Power,
                                                            Toughness  = sc.Toughness,
                                                            Rarity     = sc.Rarity,
                                                            FlavorText = sc.FlavorText,
                                                            Colors     = JsonSerializer.Serialize(sc.Colors),
                                                            Keywords   = JsonSerializer.Serialize(sc.Keywords)
                                                        }).ToList();

        await _cardRepository.AddRangeAsync(entities);
        await _cardRepository.SaveChangesAsync();

        return entities.Count;
    }

    public async Task<List<Card>> GetCardsAsync(int page = 1, int pageSize = 100) {
        var skip = (page - 1) * pageSize;
        return await _cardRepository.GetAllAsync(skip, pageSize);
    }

    public async Task<List<Card>> SearchCardsAsync(string? type, string? colors, decimal? minCmc, decimal? maxCmc,
                                                   int     take = 10) {
        return await _cardRepository.SearchAsync(type, colors, minCmc, maxCmc, take);
    }

    public async Task<Card?> GetCardAsync(int id) {
        return await _cardRepository.GetByIdAsync(id);
    }

    public async Task<int> GetTotalCountAsync() {
        return await _cardRepository.GetCountAsync();
    }

    public async Task<bool> HasDataAsync() {
        return await _cardRepository.AnyAsync();
    }
}