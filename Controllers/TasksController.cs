using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgramowanieZaawansowane.Api.Data;
using ProgramowanieZaawansowane.Api.Dtos;
using ProgramowanieZaawansowane.Api.Models;

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

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskDto dto)
    {
        var task = new TaskItem
        {
            Title = dto.Title,
            ProjectId = dto.ProjectId,
            AssignedUserId = dto.AssignedUserId
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return Ok();
    }
}