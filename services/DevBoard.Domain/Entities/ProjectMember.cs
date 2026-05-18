namespace DevBoard.Domain.Entities;

public class ProjectMember
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public ProjectRole Role { get; set; } = ProjectRole.Member;
}

public enum ProjectRole
{
    Owner, Member, Viewerd
}