using DevBoard.Application.DTOs.Tasks;
using DevBoard.Application.Interfaces.Repositories;
using MediatR;

namespace DevBoard.Application.Features.Tasks.Queries.GetTasksByBoard;

public class GetTasksByBoardHandler(ITaskRepository repository)
    : IRequestHandler<GetTasksByBoardQuery, List<TaskDto>>
{
    public async Task<List<TaskDto>> Handle(GetTasksByBoardQuery query, CancellationToken ct)
        => await repository.GetByBoardAsync(query.BoardId, ct);
}