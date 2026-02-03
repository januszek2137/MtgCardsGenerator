using AiMagicCardsGenerator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiMagicCardsGenerator.Controllers;

[Authorize(Roles = "Admin")]
public class DataController : Controller {
    private readonly ICardService     _cardService;
    private readonly IScryfallService _scryfallService;

    public DataController(ICardService cardService, IScryfallService scryfallService) {
        _cardService     = cardService;
        _scryfallService = scryfallService;
    }

    public async Task<IActionResult> Index() {
        ViewBag.BulkInfo  = await _scryfallService.GetBulkDataInfoAsync();
        ViewBag.CardCount = await _cardService.GetTotalCountAsync();
        ViewBag.HasData   = await _cardService.HasDataAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Import() {
        var count = await _cardService.ImportCardsFromScryfallAsync();
        return Json(new { imported = count });
    }
}