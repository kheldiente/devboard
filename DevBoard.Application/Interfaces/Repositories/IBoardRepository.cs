using DevBoard.Application.DTOs.Boards;

namespace DevBoard.Application.Interfaces.Repositories;

public interface IBoardRepository
{
    Task<List<BoardDto>> GetAllAsync(CancellationToken ct);
}