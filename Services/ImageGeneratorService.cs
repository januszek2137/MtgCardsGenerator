using System.Text.RegularExpressions;

namespace AiMagicCardsGenerator.Services;

public class ImageGeneratorService : IImageGeneratorService
{
    private readonly HttpClient _httpClient;

    public ImageGeneratorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<byte[]> GenerateCardArtAsync(string cardName, string typeLine, string? oracleText)
    {
        var prompt = BuildPrompt(cardName, typeLine, oracleText);
        var encodedPrompt = Uri.EscapeDataString(prompt);
        
        var url = $"https://image.pollinations.ai/prompt/{encodedPrompt}?width=3000&height=2400&nologo=true";
        
        var imageBytes = await _httpClient.GetByteArrayAsync(url);
        return imageBytes;
    }

    private string BuildPrompt(string cardName, string typeLine, string? oracleText)
    {
        // Extract creature type if exists
        var creatureType = "";
        var typeMatch = Regex.Match(typeLine, @"—\s*(.+)$");
        if (typeMatch.Success)
        {
            creatureType = typeMatch.Groups[1].Value.Trim();
        }

        var prompt = $"Fantasy art, Magic the Gathering card art style, {cardName}";
        
        if (!string.IsNullOrEmpty(creatureType))
        {
            prompt += $", {creatureType}";
        }

        if (typeLine.Contains("Creature"))
        {
            prompt += ", creature portrait, dramatic pose";
        }
        else if (typeLine.Contains("Instant") || typeLine.Contains("Sorcery"))
        {
            prompt += ", magical spell effect, energy, mystical";
        }
        else if (typeLine.Contains("Enchantment"))
        {
            prompt += ", magical aura, ethereal glow";
        }
        else if (typeLine.Contains("Artifact"))
        {
            prompt += ", magical item, detailed object";
        }
        else if (typeLine.Contains("Land"))
        {
            prompt += ", landscape, environment, scenic";
        }

        prompt += ", high fantasy, detailed, epic lighting, professional illustration";

        return prompt;
    }
}