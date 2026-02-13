namespace ProgramowanieZaawansowane.Api.Models;

public class TaskItem
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public string? AssignedUserId { get; set; }
    public ApplicationUser? AssignedUser { get; set; }
}