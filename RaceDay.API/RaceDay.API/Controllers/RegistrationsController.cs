
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistrationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public RegistrationsController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Registration>>> GetRegistrations()
    {
        return await _context.Registrations.ToListAsync();
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<Registration>> GetRegistration(int id)
    {
        var registration = await _context.Registrations.FindAsync(id);

        if (registration == null)
            return NotFound();

        return registration;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Registration>> CreateRegistration(
        Registration registration)
    {
        _context.Registrations.Add(registration);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetRegistration),
            new { id = registration.RegistrationID },
            registration);
    }

    [Authorize(Roles = "Organizer")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRegistration(
        int id, Registration registration)
    {
        if (id != registration.RegistrationID)
            return BadRequest("The ID in the URL does not match.");

        var existing = await _context.Registrations.FindAsync(id);

        if (existing == null)
            return NotFound();

        existing.Bib_Number = registration.Bib_Number;
        existing.status = registration.status;
        existing.RegistrationDate = registration.RegistrationDate;
        existing.ParticipantID = registration.ParticipantID;
        existing.RaceID = registration.RaceID;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Organizer")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRegistration(int id)
    {
        var registration = await _context.Registrations.FindAsync(id);

        if (registration == null)
            return NotFound();

        _context.Registrations.Remove(registration);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
