namespace DevBoard.Application.DTOs.Auth;

public record RegisterRequestDto(string Email, string Password, string DisplayName);
public record LoginRequestDto(string Email, string Password);
public record AuthResponseDto(string AccessToken, string RefreshToken);