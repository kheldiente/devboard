using DevBoard.Domain.Entities;

namespace DevBoard.Application.DTOs.Tasks;

public record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    TaskItemStatus Status,
    TaskItemPriority Priority,
    int StoryPoints,
    DateTime? DueDate,
    Guid? AssigneeId,
    string? AssigneeName);