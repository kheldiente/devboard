namespace DevBoard.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
    public TaskItemPriority Priority { get; set; } = TaskItemPriority.Medium;
    public DateTime? DueDate { get; set; }
    public int StoryPoints { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid BoardId { get; set; }
    public Board Board { get; set; } = null!;

    public Guid? AssigneeId { get; set; }
    public User? Assignee { get; set; }

    public void Transition(TaskItemStatus newStatus)
    {
        // domain logic - only allow valid transitions
        var allowed = Status switch
        {
            TaskItemStatus.Todo => new[] { TaskItemStatus.InProgress },
            TaskItemStatus.InProgress => new [] { TaskItemStatus.Done, TaskItemStatus.Todo },
            TaskItemStatus.Done => new [] { TaskItemStatus.Todo },
            _ => Array.Empty<TaskItemStatus>()
        };

        if (!allowed.Contains(newStatus))
            throw new InvalidOperationException($"Cannot transition from {Status} to {newStatus}");
        
        Status = newStatus;
    }
}