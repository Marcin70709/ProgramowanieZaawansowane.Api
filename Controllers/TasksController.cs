using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramowanieZaawansowane.Api.Data;
using ProgramowanieZaawansowane.Api.Dtos;
using ProgramowanieZaawansowane.Api.Models;
using System.Security.Claims;

namespace ProgramowanieZaawansowane.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TasksController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ================= GET MY TASKS =================
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetMyTasks()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var tasks = await _context.Tasks
            .Where(t =>
                t.AssignedUserId == userId ||
                t.Project.OwnerId == userId)
            .Select(t => new
            {
                t.Id,
                t.Title,
                t.ProjectId,
                t.AssignedUserId
            })
            .ToListAsync();

        return Ok(tasks);
    }

    // ================= GET BY ID =================
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var task = await _context.Tasks
            .Where(t =>
                t.Id == id &&
                (t.AssignedUserId == userId ||
                 t.Project.OwnerId == userId))
            .Select(t => new
            {
                t.Id,
                t.Title,
                t.ProjectId,
                t.AssignedUserId
            })
            .FirstOrDefaultAsync();

        if (task == null)
            return NotFound();

        return Ok(task);
    }

    // ================= CREATE =================
    [HttpPost]
    public async Task<ActionResult> Create(CreateTaskDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == dto.ProjectId);

        if (project == null)
            return NotFound("Project not found");

        if (project.OwnerId != userId)
            return Forbid();

        var task = new TaskItem
        {
            Title = dto.Title,
            ProjectId = dto.ProjectId,
            AssignedUserId = dto.AssignedUserId
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    // ================= UPDATE =================
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateTaskDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
            return NotFound();

        if (task.Project.OwnerId != userId)
            return Forbid();

        task.Title = dto.Title;
        task.AssignedUserId = dto.AssignedUserId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ================= DELETE =================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
            return NotFound();

        if (task.Project.OwnerId != userId)
            return Forbid();

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}