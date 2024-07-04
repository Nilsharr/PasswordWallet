using Core.Models;

namespace Core.Interfaces.Services;

public interface ITokenService
{
    TokenResult GenerateJwtToken(long userId);
    TokenResult GenerateRefreshToken();
    Task<TokenResult?> RefreshAccessToken(string accessToken, string refreshToken, CancellationToken ct = default);
}