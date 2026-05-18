using DevBoard.Application.DTOs.Tasks;
using MediatR;

namespace DevBoard.Application.Features.Tasks.Queries.GetTasksByBoard;

public record GetTasksByBoardQuery(Guid BoardId) : IRequest<List<TaskDto>>;