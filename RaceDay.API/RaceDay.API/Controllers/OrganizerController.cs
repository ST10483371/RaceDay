
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrganizersController(AppDbContext context)
    {
        _context = context;
    }

    // Only authenticated users can view organizers
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Organizer>>> GetOrganizers()
    {
        return await _context.Organizers.ToListAsync();
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<Organizer>> GetOrganizer(int id)
    {
        var organizer = await _context.Organizers.FindAsync(id);

        if (organizer == null)
            return NotFound();

        return organizer;
    }

    // Only organizers can create, update or delete organizer records
    [Authorize(Roles = "Organizer")]
    [HttpPost]
    public async Task<ActionResult<Organizer>> CreateOrganizer(Organizer organizer)
    {
        _context.Organizers.Add(organizer);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetOrganizer),
            new { id = organizer.OrganizerID },
            organizer);
    }

    [Authorize(Roles = "Organizer")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrganizer(
        int id, Organizer organizer)
    {
        if (id != organizer.OrganizerID)
            return BadRequest("The ID in the URL does not match.");

        var existing = await _context.Organizers.FindAsync(id);

        if (existing == null)
            return NotFound();

        existing.Name = organizer.Name;
        existing.Contact_Number = organizer.Contact_Number;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Organizer")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrganizer(int id)
    {
        var organizer = await _context.Organizers.FindAsync(id);

        if (organizer == null)
            return NotFound();

        _context.Organizers.Remove(organizer);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
