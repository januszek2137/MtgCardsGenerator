using AiMagicCardsGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiMagicCardsGenerator.Controllers;

public class CardsController : Controller {
    private readonly ICardService _cardService;

    public CardsController(ICardService cardService) {
        _cardService = cardService;
    }

    public async Task<IActionResult> Index(int page = 1) {
        var cards = await _cardService.GetCardsAsync(page, 20);
        ViewBag.Page       = page;
        ViewBag.TotalCount = await _cardService.GetTotalCountAsync();
        return View(cards);
    }

    public async Task<IActionResult> Details(int id) {
        var card = await _cardService.GetCardAsync(id);
        if (card == null) return NotFound();
        return View(card);
    }
}