using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using AiMagicCardsGenerator.Models;
using AiMagicCardsGenerator.Repositories;
using AiMagicCardsGenerator.Services;
using Microsoft.AspNetCore.Authorization;

namespace AiMagicCardsGenerator.Controllers;

public class HomeController : Controller
{
    private readonly IGeneratedCardRepository _generatedCardRepository;
    private readonly ICardLikeService _likeService;
    private readonly ILikesBroadcastService _broadcast;

    public HomeController(
        IGeneratedCardRepository generatedCardRepository,
        ICardLikeService likeService,
        ILikesBroadcastService broadcast)
    {
        _generatedCardRepository = generatedCardRepository;
        _likeService = likeService;
        _broadcast = broadcast;
    }

    public async Task<IActionResult> Index()
    {
        var recentCards = await _generatedCardRepository.GetSharedAsync(12);
        var topCards = await _generatedCardRepository.GetTopLikedAsync(6);

        ViewBag.TopCards = topCards;

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != null)
        {
            var allCardIds = recentCards.Select(c => c.Id).Concat(topCards.Select(c => c.Id));
            ViewBag.UserLikes = await _likeService.GetUserLikesAsync(userId, allCardIds);
        }
        else
        {
            ViewBag.UserLikes = new HashSet<int>();
        }

        return View(recentCards);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Like(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        var result = await _likeService.ToggleAsync(id, userId);

        if (!result.Success)
            return NotFound();

        return Json(new { result.Likes, result.IsLiked });
    }

    [HttpGet]
    public async Task LikeStream()
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";

        var connectionId = Guid.NewGuid().ToString();
        var writer = new StreamWriter(Response.Body);
        _broadcast.Subscribe(connectionId, writer);

        try
        {
            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                await Task.Delay(30000, HttpContext.RequestAborted);
                await writer.WriteAsync(": ping\n\n");
                await writer.FlushAsync();
            }
        }
        catch (OperationCanceledException) { }
        finally { _broadcast.Unsubscribe(connectionId); }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}