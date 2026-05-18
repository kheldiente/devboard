using DevBoard.Domain.Entities;

namespace DevBoard.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}