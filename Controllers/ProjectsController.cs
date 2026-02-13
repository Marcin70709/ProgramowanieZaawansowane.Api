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

    // GET: api/projects
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

    // POST: api/projects
    [HttpPost]
    public async Task<ActionResult<ProjectDto>> CreateProject(CreateProjectDto dto)
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

        return Ok(result);
    }
}