using System.Text.Json.Serialization;

namespace AiMagicCardsGenerator.Models.Dto;

public class ScryfallBulkDataResponse {
    public List<ScryfallBulkDataItem> Data { get; set; } = [];
}

public class ScryfallBulkDataItem {
    public string Type { get; set; } = "";

    [JsonPropertyName("download_uri")] public string DownloadUri { get; set; } = "";

    public long Size { get; set; }

    [JsonPropertyName("updated_at")] public DateTime UpdatedAt { get; set; }
}

public class ScryfallCard {
    public string Id   { get; set; } = "";
    public string Name { get; set; } = "";

    [JsonPropertyName("mana_cost")] public string? ManaCost { get; set; }

    public decimal Cmc { get; set; }

    [JsonPropertyName("type_line")] public string TypeLine { get; set; } = "";

    [JsonPropertyName("oracle_text")] public string? OracleText { get; set; }

    public string?      Power     { get; set; }
    public string?      Toughness { get; set; }
    public List<string> Colors    { get; set; } = [];
    public List<string> Keywords  { get; set; } = [];
    public string       Rarity    { get; set; } = "";

    [JsonPropertyName("flavor_text")] public string? FlavorText { get; set; }

    public string? Layout { get; set; }
}