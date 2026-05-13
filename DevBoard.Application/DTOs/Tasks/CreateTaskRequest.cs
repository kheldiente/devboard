namespace DevBoard.Application.DTOs.Tasks;

public record CreateTaskRequest(
    string Title,
    string? Description,
    Guid BoardId,
    Guid? AssigneeId,
    int StoryPoints);