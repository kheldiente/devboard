using DevBoard.Application.DTOs.Boards;
using DevBoard.Application.Features.Boards.Queries.GetAllBoards;
using DevBoard.Application.Interfaces.Repositories;
using MediatR;

namespace DevBoard.Application.Features.boards.Queries.GetAllBoards;

public class GetAllBoardsHandler(IBoardRepository repository)
    : IRequestHandler<GetAllBoardsQuery, List<BoardDto>>
{
    public async Task<List<BoardDto>> Handle(GetAllBoardsQuery query, CancellationToken ct)
        => await repository.GetAllAsync(ct);
}