using DevBoard.Application.DTOs.Tasks;
using DevBoard.Domain.Entities;

namespace DevBoard.Application.Interfaces.Repositories;

public interface ITaskRepository
{
    Task<Guid> CreateAsync(TaskItem task, CancellationToken ct);
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<TaskDto>> GetByBoardAsync(Guid boardId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}