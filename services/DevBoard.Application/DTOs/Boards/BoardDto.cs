namespace DevBoard.Application.DTOs.Boards;

public record BoardDto(Guid Id, string Name, Guid ProjectId);