using DevBoard.Application.DTOs.Tasks;
using DevBoard.Application.Interfaces.Repositories;
using DevBoard.Domain.Entities;
using DevBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevBoard.Infrastructure.Repositories;

public class TaskRepository(AppDbContext db) : ITaskRepository
{
    public async Task<Guid> CreateAsync(TaskItem task, CancellationToken ct)
    {
        db.Tasks.Add(task);
        await db.SaveChangesAsync(ct);
        return task.Id;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct)
        => await db.Tasks.SingleOrDefaultAsync(t => t.Id == id, ct);

    public async Task<List<TaskDto>> GetByBoardAsync(Guid boardId, CancellationToken ct)
        => await db.Tasks
            .Where(t => t.BoardId == boardId)
            .Include(t => t.Assignee)
            .Select(t => new TaskDto(
                t.Id,
                t.Title,
                t.Description,
                t.Status,
                t.Priority,
                t.StoryPoints,
                t.DueDate,
                t.AssigneeId,
                t.Assignee != null ? t.Assignee.DisplayName : null))
            .ToListAsync(ct);
    
    public async Task SaveChangesAsync(CancellationToken ct)
        => await db.SaveChangesAsync(ct);
}