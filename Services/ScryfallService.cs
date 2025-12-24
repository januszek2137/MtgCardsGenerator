using System.Text.Json;
using AiMagicCardsGenerator.Models.Dto;

namespace AiMagicCardsGenerator.Services;

public class ScryfallService : IScryfallService {
    private readonly HttpClient _httpClient;

    public ScryfallService(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    public async Task<ScryfallBulkDataItem?> GetBulkDataInfoAsync() {
        var response = await _httpClient.GetFromJsonAsync<ScryfallBulkDataResponse>("bulk-data");
        return response?.Data.FirstOrDefault(d => d.Type == "all_cards");
    }

    public async Task<List<ScryfallCard>> DownloadAllCardsAsync() {
        var bulkInfo = await GetBulkDataInfoAsync()
            ?? throw new Exception("Nie można pobrać informacji o bulk data");

        using var response = await _httpClient.GetAsync(bulkInfo.DownloadUri,
            HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        var cards = await JsonSerializer.DeserializeAsync<List<ScryfallCard>>(stream,
            new JsonSerializerOptions
                { PropertyNameCaseInsensitive = true });

        return cards ?? [];
    }

}