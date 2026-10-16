
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResultsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ResultsController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Result>>> GetResults()
    {
        return await _context.Results.ToListAsync();
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<Result>> GetResult(int id)
    {
        var result = await _context.Results.FindAsync(id);

        if (result == null)
            return NotFound();

        return result;
    }

    [Authorize(Roles = "Organizer")]
    [HttpPost]
    public async Task<ActionResult<Result>> CreateResult(Result result)
    {
        _context.Results.Add(result);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetResult),
            new { id = result.ResultID },
            result);
    }

    [Authorize(Roles = "Organizer")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateResult(int id, Result result)
    {
        if (id != result.ResultID)
            return BadRequest("The ID in the URL does not match.");

        var existing = await _context.Results.FindAsync(id);

        if (existing == null)
            return NotFound();

        existing.RegistrationID = result.RegistrationID;
        existing.Finish_Time = result.Finish_Time;
        existing.Position = result.Position;
        existing.Status = result.Status;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Organizer")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteResult(int id)
    {
        var result = await _context.Results.FindAsync(id);

        if (result == null)
            return NotFound();

        _context.Results.Remove(result);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
