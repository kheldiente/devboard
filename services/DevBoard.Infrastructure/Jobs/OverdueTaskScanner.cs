using DevBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevBoard.Infrastructure.Jobs;

public class OverdueTaskScanner(AppDbContext db, ILogger<OverdueTaskScanner> logger)
{
    public async Task RunAsync()
    {
        var overdueTasks = await db.Tasks
            .Where(t => t.DueDate < DateTime.UtcNow
                && t.Status != Domain.Entities.TaskItemStatus.Done)
            .ToListAsync();

        if (overdueTasks.Count == 0)
        {
            logger.LogInformation("No overdue tasks found.");
            return;
        }

        logger.LogWarning("{Count} overdue tasks(s) found:", overdueTasks.Count);

        foreach (var task in overdueTasks)
            logger.LogWarning(" - [{Id} {Title} was due {DueDate}]", task.Id, task.Title, task.DueDate);
    }
}