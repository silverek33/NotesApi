
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesApi.Data;
using NotesApi.Models;

namespace NotesApi.Controllers;

[ApiController]
[Authorize]
[Route("notes")]
public class NotesController : ControllerBase
{
    private readonly AppDbContext _db;
    public NotesController(AppDbContext db) { _db = db; }

    public record NoteDto(int Id, string Content);
    public record CreateNoteRequest(string Content);
    public record UpdateNoteRequest(string Content);

    private bool TryGetUserId(out Guid userId)
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idStr, out userId);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NoteDto>>> GetAll()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var notes = await _db.Notes.AsNoTracking()
            .Where(n => n.UserId == userId)
            .Select(n => new NoteDto(n.Id, n.Content)).ToListAsync();
        return Ok(notes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<NoteDto>> GetById(int id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var note = await _db.Notes.AsNoTracking()
            .Where(n => n.UserId == userId && n.Id == id)
            .Select(n => new NoteDto(n.Id, n.Content))
            .FirstOrDefaultAsync();
        return note is null ? NotFound() : Ok(note);
    }

    [HttpPost]
    public async Task<ActionResult<NoteDto>> Create([FromBody] CreateNoteRequest req)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        if (string.IsNullOrWhiteSpace(req.Content)) return BadRequest("Content jest wymagany.");
        var note = new Note { Content = req.Content, UserId = userId };
        _db.Notes.Add(note);
        await _db.SaveChangesAsync();
        var dto = new NoteDto(note.Id, note.Content);
        return CreatedAtAction(nameof(GetById), new { id = note.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<NoteDto>> Update(int id, [FromBody] UpdateNoteRequest req)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        if (string.IsNullOrWhiteSpace(req.Content)) return BadRequest("Content jest wymagany.");
        var note = await _db.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
        if (note is null) return NotFound();
        note.Content = req.Content;
        await _db.SaveChangesAsync();
        return Ok(new NoteDto(note.Id, note.Content));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var note = await _db.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
        if (note is null) return NotFound();
        _db.Notes.Remove(note);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
