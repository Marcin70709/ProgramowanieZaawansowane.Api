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
public class ProjectsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProjectsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ================= GET ALL (MOJE PROJEKTY) =================
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetMyProjects()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var projects = await _context.Projects
            .Where(p =>
                p.OwnerId == userId ||
                p.Tasks.Any(t => t.AssignedUserId == userId))
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name
            })
            .ToListAsync();

        return Ok(projects);
    }

    // ================= GET BY ID =================
    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var project = await _context.Projects
            .Include(p => p.Tasks)
            .Where(p =>
                p.Id == id &&
                (p.OwnerId == userId ||
                 p.Tasks.Any(t => t.AssignedUserId == userId)))
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name
            })
            .FirstOrDefaultAsync();

        if (project == null)
            return NotFound();

        return Ok(project);
    }

    // ================= CREATE =================
    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(CreateProjectDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var project = new Project
        {
            Name = dto.Name,
            OwnerId = userId!
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var result = new ProjectDto
        {
            Id = project.Id,
            Name = project.Name
        };

        return CreatedAtAction(nameof(GetById), new { id = project.Id }, result);
    }

    // ================= UPDATE =================
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateProjectDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
            return NotFound();

        if (project.OwnerId != userId)
            return Forbid();

        project.Name = dto.Name;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ================= DELETE =================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var project = await _context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
            return NotFound();

        if (project.OwnerId != userId)
            return Forbid();

        _context.Tasks.RemoveRange(project.Tasks);
        _context.Projects.Remove(project);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}