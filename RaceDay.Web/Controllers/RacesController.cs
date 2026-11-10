using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers;

public class RacesController : Controller
{
    private readonly RaceDayApiClient _api;

    public RacesController(RaceDayApiClient api)
    {
        _api = api;
    }

    private string? Token => HttpContext.Session.GetString(SessionKeys.Token);
    private bool IsOrganizer => HttpContext.Session.GetString(SessionKeys.Role) == "Organizer";

    // GET: /Races
    public async Task<IActionResult> Index()
    {
        var result = await _api.GetRacesAsync();

        ViewBag.IsOrganizer = IsOrganizer;
        ViewBag.Error = result.Error;

        return View(result.Data ?? new List<Race>());
    }

    // GET: /Races/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var result = await _api.GetRaceAsync(id);

        if (!result.Success || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "That race could not be found.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.IsOrganizer = IsOrganizer;
        return View(result.Data);
    }

    // GET: /Races/Create
    public IActionResult Create()
    {
        if (!IsOrganizer)
            return NotOrganizer();

        return View(new Race());
    }

    // POST: /Races/Create
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Race race)
    {
        if (!IsOrganizer)
            return NotOrganizer();

        if (!ModelState.IsValid)
            return View(race);

        var result = await _api.CreateRaceAsync(race, Token!);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "The race could not be created.");
            return View(race);
        }

        TempData["Success"] = "Race created.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Races/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsOrganizer)
            return NotOrganizer();

        var result = await _api.GetRaceAsync(id);

        if (!result.Success || result.Data is null)
        {
            TempData["Error"] = result.Error ?? "That race could not be found.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    // POST: /Races/Edit/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Race race)
    {
        if (!IsOrganizer)
            return NotOrganizer();

        race.RaceID = id;

        if (!ModelState.IsValid)
            return View(race);

        var result = await _api.UpdateRaceAsync(race, Token!);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "The race could not be updated.");
            return View(race);
        }

        TempData["Success"] = "Race updated.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Races/Delete/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsOrganizer)
            return NotOrganizer();

        var result = await _api.DeleteRaceAsync(id, Token!);

        if (result.Success)
            TempData["Success"] = "Race deleted.";
        else
            TempData["Error"] = result.Error;

        return RedirectToAction(nameof(Index));
    }

    private IActionResult NotOrganizer()
    {
        TempData["Error"] = "Only organizers can do that. Log in with an organizer account.";
        return RedirectToAction(nameof(Index));
    }
}
