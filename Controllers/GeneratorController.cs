using AiMagicCardsGenerator.Models.Entities;
using AiMagicCardsGenerator.Repositories;
using AiMagicCardsGenerator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace AiMagicCardsGenerator.Controllers;

public class GeneratorController : Controller {
    private readonly IGeneratorService _generatorService;
    private readonly ICardRenderService _cardRenderService;
    private readonly IGeneratedCardRepository _generatedCardRepository;

    public GeneratorController(
        IGeneratorService generatorService,
        ICardRenderService cardRenderService,
        IGeneratedCardRepository generatedCardRepository)
    {
        _generatorService = generatorService;
        _cardRenderService = cardRenderService;
        _generatedCardRepository = generatedCardRepository;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [EnableRateLimiting("GeneratorLimit")]
    public async Task<IActionResult> Generate(int cardCmc)
    {
        var result = await _generatorService.GenerateCardWithConvertedManaCostAsync(cardCmc);
        var imageBytes = await _cardRenderService.RenderCardAsync(result.Card);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var generatedCard = new GeneratedCard
        {
            Name = result.Card.Name,
            ManaCost = result.Card.ManaCost,
            Cmc = result.Card.Cmc,
            TypeLine = result.Card.TypeLine,
            OracleText = result.Card.OracleText,
            Power = result.Card.Power,
            Toughness = result.Card.Toughness,
            Colors = result.Card.Colors,
            FlavorText = result.Card.FlavorText,
            ImageData = imageBytes,
            CreatedBy = userId ?? "Anonymous",
            CreatorIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown"
        };

        await _generatedCardRepository.AddAsync(generatedCard);

        return RedirectToAction("Result", new { id = generatedCard.Id });
    }

    public async Task<IActionResult> Result(int id)
    {
        var card = await _generatedCardRepository.GetByIdAsync(id);
        if (card == null)
            return RedirectToAction("Index");

        return View(card);
    }

    [HttpPost]
    public async Task<IActionResult> Share(int id)
    {
        var card = await _generatedCardRepository.GetByIdAsync(id);
        if (card == null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (card.CreatedBy != userId)
            return Forbid();

        await _generatedCardRepository.ShareAsync(id);
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Image(int id)
    {
        var card = await _generatedCardRepository.GetByIdAsync(id);
        if (card?.ImageData == null)
            return NotFound();

        return File(card.ImageData, "image/png");
    }

    public async Task<IActionResult> Download(int id)
    {
        var card = await _generatedCardRepository.GetByIdAsync(id);
        if (card?.ImageData == null)
            return NotFound();

        return File(card.ImageData, "image/png", $"{card.Name}.png");
    }
}