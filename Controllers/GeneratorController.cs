using AiMagicCardsGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiMagicCardsGenerator.Controllers;

public class GeneratorController : Controller {
    private readonly IGeneratorService  _generatorService;
    private readonly ICardRenderService _cardRenderService;

    public GeneratorController(IGeneratorService generatorService, ICardRenderService cardRenderService) {
        _generatorService = generatorService;
        _cardRenderService = cardRenderService;
    }

    public IActionResult Index() {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Generate() {
        var result = await _generatorService.GenerateRandomCardAsync();
        return Json(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> GenerateImage()
    {
        var result     = await _generatorService.GenerateRandomCardAsync();
        var imageBytes = await _cardRenderService.RenderCardAsync(result.Card);
        return File(imageBytes, "image/png");
    }
}