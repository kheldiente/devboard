using DevBoard.Application.DTOs.Boards;
using DevBoard.Application.Interfaces.Repositories;
using DevBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevBoard.Infrastructure.Repositories;

public class BoardRepository(AppDbContext db) : IBoardRepository
{
    public async Task<List<BoardDto>> GetAllAsync(CancellationToken ct)
        => await db.Boards
            .Select(b => new BoardDto(b.Id, b.Name, b.ProjectId))
            .ToListAsync(ct);
}