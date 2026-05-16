using DevBoard.Application.DTOs.Boards;
using MediatR;

namespace DevBoard.Application.Features.Boards.Queries.GetAllBoards;

public record GetAllBoardsQuery : IRequest<List<BoardDto>>;