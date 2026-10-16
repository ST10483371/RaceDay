
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RacesController : ControllerBase
{
    private readonly AppDbContext _context;

    public RacesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/races
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Race>>> GetRaces()
    {
        return await _context.Races.ToListAsync();
    }

    // GET: api/races/1
    [HttpGet("{id}")]
    public async Task<ActionResult<Race>> GetRace(int id)
    {
        var race = await _context.Races.FindAsync(id);

        if (race == null)
            return NotFound();

        return race;
    }

    // POST: api/races
    [Authorize(Roles = "Organizer")]
    [HttpPost]
    public async Task<ActionResult<Race>> CreateRace(Race race)
    {
        _context.Races.Add(race);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetRace),
            new { id = race.RaceID },
            race);
    }

    // PUT: api/races/1
    [Authorize(Roles = "Organizer")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRace(int id, Race race)
    {
        if (id != race.RaceID)
            return BadRequest("The ID in the URL does not match the race.");

        var existing = await _context.Races.FindAsync(id);

        if (existing == null)
            return NotFound();

        existing.Name = race.Name;
        existing.Location = race.Location;
        existing.StartTime = race.StartTime;
        existing.EndTime = race.EndTime;
        existing.Distance_km = race.Distance_km;
        existing.Type = race.Type;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/races/1
    [Authorize(Roles = "Organizer")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRace(int id)
    {
        var race = await _context.Races.FindAsync(id);

        if (race == null)
            return NotFound();

        _context.Races.Remove(race);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
