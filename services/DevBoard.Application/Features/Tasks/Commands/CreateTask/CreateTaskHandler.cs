using DevBoard.Application.Interfaces.Repositories;
using DevBoard.Domain.Entities;
using MediatR;

namespace DevBoard.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskHandler(ITaskRepository repository) : IRequestHandler<CreateTaskCommand, Guid>
{
    public async Task<Guid> Handle(CreateTaskCommand command, CancellationToken ct)
    {
        var task = new TaskItem
        {
            Title = command.Title,
            Description = command.Description,
            BoardId = command.BoardId,
            AssigneeId = command.AssigneeId,
            StoryPoints = command.StoryPoints
        };

        return await repository.CreateAsync(task, ct);
    }
}