namespace DevBoard.Domain.Entities;

public class Board
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public ICollection<TaskItem> Tasks { get; set; } = [];
}