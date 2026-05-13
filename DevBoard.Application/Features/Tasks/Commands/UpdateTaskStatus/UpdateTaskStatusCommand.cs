using DevBoard.Domain.Entities;
using MediatR;

namespace DevBoard.Application.Features.Tasks.Commands.UpdateTaskStatus;

public record UpdateTaskStatusCommand(Guid TaskId, TaskItemStatus NewStatus) : IRequest;