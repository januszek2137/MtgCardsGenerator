using System.Text;
using System.Text.Json;
using AiMagicCardsGenerator.Models.Entities;
using AiMagicCardsGenerator.Repositories;

namespace AiMagicCardsGenerator.Services;

public class GeneratorService : IGeneratorService {
    private readonly ICardRepository _cardRepository;
    private readonly HttpClient      _httpClient;
    private readonly IConfiguration  _configuration;

    public GeneratorService(ICardRepository cardRepository, HttpClient httpClient, IConfiguration configuration) {
        _cardRepository = cardRepository;
        _httpClient     = httpClient;
        _configuration  = configuration;
    }

    public async Task<CardGenerationResult> GenerateCardWithConvertedManaCostAsync(int targetCmc) {
        var examples = await _cardRepository.GetRandomByCmcAsync(targetCmc, 5);
        var prompt   = BuildPrompt(examples, targetCmc);
        var response = await CallGroqAsync(prompt);
        var card     = ParseResponse(response);

        return new CardGenerationResult
        {
            Card    = card,
            BasedOn = examples
        };
    }
    
    
    public async Task<CardGenerationResult> GenerateRandomCardAsync()
    {
        var random    = new Random();
        var targetCmc = random.Next(1, 8);
    
        var examples = await _cardRepository.GetRandomByCmcAsync(targetCmc, 5);
        var prompt   = BuildPrompt(examples, targetCmc);
        var response = await CallGroqAsync(prompt);
        var card     = ParseResponse(response);

        return new CardGenerationResult
        {
            Card    = card,
            BasedOn = examples
        };
    }

    private string BuildPrompt(List<Card> examples, int targetCmc) {
        var sb = new StringBuilder();
    
        sb.AppendLine("You are an expert Magic: The Gathering card designer.");
        sb.AppendLine($"Create a NEW card with converted mana cost (CMC) of exactly {targetCmc}.");
        sb.AppendLine("Base your design on these example cards:");
        sb.AppendLine();
        sb.AppendLine("=== EXAMPLE CARDS ===");

        foreach (var card in examples) {
            sb.AppendLine($"Name: {card.Name}");
            sb.AppendLine($"Mana Cost: {card.ManaCost}");
            sb.AppendLine($"Type: {card.TypeLine}");
            sb.AppendLine($"Text: {card.OracleText}");
            if (card.Power != null)
                sb.AppendLine($"Power/Toughness: {card.Power}/{card.Toughness}");
            sb.AppendLine();
        }

        sb.AppendLine("=== TASK ===");
        sb.AppendLine(
            "Create a new card inspired by the above. It can be any type (creature, instant, sorcery, enchantment, artifact).");
        sb.AppendLine();
        sb.AppendLine("Respond ONLY with JSON, no additional text:");
        sb.AppendLine("""
                      {
                          "name": "card name",
                          "manaCost": "{mana symbols}",
                          "typeLine": "full type line",
                          "oracleText": "card rules text",
                          "power": "only for creatures, otherwise null",
                          "toughness": "only for creatures, otherwise null",
                          "flavorText": "short flavor text"
                      }
                      """);

        return sb.ToString();
    }

    private async Task<string> CallGroqAsync(string prompt)
    {
        var apiKey = _configuration["Groq:ApiKey"] 
            ?? throw new InvalidOperationException("Missing Groq API key in configuration");

        var requestBody = new
        {
            model = "llama-3.3-70b-versatile",
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            temperature = 0.8,
            max_tokens  = 500
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions")
        {
            Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Authorization", $"Bearer {apiKey}");

        var response = await _httpClient.SendAsync(request);
    
        var responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Groq API error: {response.StatusCode} - {responseBody}");
        }

        var doc = JsonDocument.Parse(responseBody);
    
        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "";
    }

    private Card ParseResponse(string response) {
        var start = response.IndexOf('{');
        var end   = response.LastIndexOf('}');

        if (start == -1 || end == -1)
            throw new InvalidOperationException("Failed to parse LLM response");

        var json = response.Substring(start, end - start + 1);

        return JsonSerializer.Deserialize<Card>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Failed to deserialize card");
    }
}