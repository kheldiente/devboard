using MediatR;

namespace DevBoard.Application.Features.Tasks.Commands.CreateTask;

public record CreateTaskCommand(
    string Title,
    string? Description,
    Guid BoardId,
    Guid? AssigneeId,
    int StoryPoints) : IRequest<Guid>;