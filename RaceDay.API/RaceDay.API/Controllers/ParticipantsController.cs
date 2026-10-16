
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParticipantsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ParticipantsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/participants
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Participant>>> GetParticipants()
    {
        return await _context.Participants.ToListAsync();
    }

    // GET: api/participants/1
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<Participant>> GetParticipant(int id)
    {
        var participant = await _context.Participants.FindAsync(id);

        if (participant == null)
            return NotFound();

        return participant;
    }

    // POST: api/participants
    [Authorize(Roles = "Organizer")]
    [HttpPost]
    public async Task<ActionResult<Participant>> CreateParticipant(
        Participant participant)
    {
        _context.Participants.Add(participant);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetParticipant),
            new { id = participant.ParticipantID },
            participant);
    }

    // PUT: api/participants/1
    [Authorize(Roles = "Organizer")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateParticipant(
        int id, Participant participant)
    {
        if (id != participant.ParticipantID)
            return BadRequest("The ID in the URL does not match.");

        var existing = await _context.Participants.FindAsync(id);

        if (existing == null)
            return NotFound();

        existing.Name = participant.Name;
        existing.Gender = participant.Gender;
        existing.EmergencyContact = participant.EmergencyContact;
        existing.TeamName = participant.TeamName;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/participants/1
    [Authorize(Roles = "Organizer")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteParticipant(int id)
    {
        var participant = await _context.Participants.FindAsync(id);

        if (participant == null)
            return NotFound();

        _context.Participants.Remove(participant);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
