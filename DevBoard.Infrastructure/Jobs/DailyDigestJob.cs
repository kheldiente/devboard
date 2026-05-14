using DevBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevBoard.Infrastructure.Jobs;

public class DailyDigestJob(AppDbContext db, ILogger<DailyDigestJob> logger)
{
    public async Task RunAsync()
    {
        var stats = await db.Tasks
            .GroupBy(t => t.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        logger.LogInformation("=== Daily Task Digest ===");

        foreach (var stat in stats)
            logger.LogInformation("  {Status}: {Count} task(s)", stat.Status, stat.Count);
    }
}