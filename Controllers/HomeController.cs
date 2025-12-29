using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AiMagicCardsGenerator.Models;
using AiMagicCardsGenerator.Repositories;

namespace AiMagicCardsGenerator.Controllers;

public class HomeController : Controller {
    private readonly IGeneratedCardRepository _generatedCardRepository;

    public HomeController(IGeneratedCardRepository generatedCardRepository) {
        _generatedCardRepository = generatedCardRepository;
    }

    public async Task<IActionResult> Index() {
        var recentCards = await _generatedCardRepository.GetSharedAsync(12);
        var topCards    = await _generatedCardRepository.GetTopLikedAsync(6);

        ViewBag.TopCards = topCards;
        return View(recentCards);
    }

    [HttpPost]
    public async Task<IActionResult> Like(int id) {
        await _generatedCardRepository.IncrementLikesAsync(id);
        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}