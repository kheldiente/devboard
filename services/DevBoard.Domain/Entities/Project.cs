namespace DevBoard.Domain.Entities;
public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Board> Boards { get; set; } = [];
    public ICollection<ProjectMember> Members { get; set; } = [];
}