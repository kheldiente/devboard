using DevBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevBoard.Infrastructure.Persistence;

public static class Seed
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Projects.AnyAsync())
            return; // already seeded

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "DevBoard Demo Project"
        };

        var board = new Board
        {
            Id = Guid.NewGuid(),
            Name = "Sprint 1",
            ProjectId = project.Id
        };
        
        db.Projects.Add(project);
        db.Boards.Add(board);
        await db.SaveChangesAsync();

        Console.WriteLine($"Seeded Project: {project.Id}");
        Console.WriteLine($"Seeded Board: {board.Id}");
    }
}