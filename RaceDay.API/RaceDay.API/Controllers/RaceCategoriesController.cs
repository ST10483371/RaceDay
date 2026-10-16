
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RaceCategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public RaceCategoriesController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RaceCategory>>> GetRaceCategories()
    {
        return await _context.RaceCategories.ToListAsync();
    }

    [Authorize]
    [HttpGet("{raceId}/{categoryId}")]
    public async Task<ActionResult<RaceCategory>> GetRaceCategory(
        int raceId, int categoryId)
    {
        var item = await _context.RaceCategories.FindAsync(raceId, categoryId);

        if (item == null)
            return NotFound();

        return item;
    }

    [Authorize(Roles = "Organizer")]
    [HttpPost]
    public async Task<ActionResult<RaceCategory>> CreateRaceCategory(
        RaceCategory item)
    {
        if (item.RaceID == null || item.CategoryID == null)
            return BadRequest("RaceID and CategoryID are required.");

        _context.RaceCategories.Add(item);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetRaceCategory),
            new { raceId = item.RaceID, categoryId = item.CategoryID },
            item);
    }

    [Authorize(Roles = "Organizer")]
    [HttpDelete("{raceId}/{categoryId}")]
    public async Task<IActionResult> DeleteRaceCategory(
        int raceId, int categoryId)
    {
        var item = await _context.RaceCategories.FindAsync(raceId, categoryId);

        if (item == null)
            return NotFound();

        _context.RaceCategories.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
