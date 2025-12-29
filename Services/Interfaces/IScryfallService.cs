using AiMagicCardsGenerator.Models.Dto;

namespace AiMagicCardsGenerator.Services;

public interface IScryfallService {
    Task<ScryfallBulkDataItem?> GetBulkDataInfoAsync();
    Task<List<ScryfallCard>>    DownloadAllCardsAsync();
}