namespace ProgramowanieZaawansowane.Api.Dtos;

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public string AssignedUserId { get; set; } = string.Empty;
}