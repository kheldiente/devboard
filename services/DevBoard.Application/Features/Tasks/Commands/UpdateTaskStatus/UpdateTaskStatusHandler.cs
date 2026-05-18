using DevBoard.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevBoard.Application.Features.Tasks.Commands.UpdateTaskStatus;

public class UpdateTaskStatusHandler(ITaskRepository repository) : IRequestHandler<UpdateTaskStatusCommand>
{
    public async Task Handle(UpdateTaskStatusCommand command, CancellationToken ct)
    {
        var task = await repository.GetByIdAsync(command.TaskId, ct)
            ?? throw new KeyNotFoundException($"Task {command.TaskId} not found");

        task.Transition(command.NewStatus); // domain logic handles validation
        await repository.SaveChangesAsync(ct);
    }
}