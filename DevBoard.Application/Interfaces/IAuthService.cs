using DevBoard.Application.DTOs.Auth;

namespace DevBoard.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsnyc(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> RefreshAsync(string refreshToken);
    Task RevokeAsync(string refreshToken);
}