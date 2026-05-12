using DevBoard.Application.DTOs.Auth;
using DevBoard.Application.Interfaces;
using DevBoard.Domain.Entities;
using DevBoard.Infrastructure.Persisttence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DevBoard.Infrastructure.Services;

public class AuthService(
    UserManager<User> userManager,
    AppDbContext db,
    ITokenService tokenService) : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var user = new User
        {
            Email = request.Email,
            UserName = request.Email,
            DisplayName = request.DisplayName
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        return await IssueTokenAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("Invalid credentials");
        
        var valid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!valid)
            throw new UnauthorizedAccessException("Invalid credentials");

        return await IssueTokenAsync(user);
    }

    public async Task<AuthResponseDto> RefreshAsync(string refreshToken)
    {
        var token = await db.RefreshTokens
            .Include(r => r.User)
            .SingleOrDefaultAsync(r => r.Token == refreshToken) ?? throw new UnauthorizedAccessException("Invalid refresh token");

        if (token.IsRevoked || token.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token expired or revoked");

        // rotate - invalidate old, issue new
        token.IsRevoked = true;
        await db.SaveChangesAsync();

        return await IssueTokenAsync(token.User);
    }

    public async Task RevokeAsync(string refreshToken)
    {
        var token = await db.RefreshTokens
            .SingleOrDefaultAsync(r => r.Token == refreshToken);

        if (token is null)
            return;

        token.IsRevoked = true;
        await db.SaveChangesAsync();
    }

    private async Task<AuthResponseDto> IssueTokenAsync(User user)
    {
        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        db.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        await db.SaveChangesAsync();

        return new AuthResponseDto(accessToken, refreshToken);
    }
}