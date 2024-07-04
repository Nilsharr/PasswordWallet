using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Core.Constants;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using FastEndpoints.Security;
using Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Identity;

public class TokenService(
    IUnitOfWork unitOfWork,
    IOptions<JwtOptions> jwtOptions,
    TimeProvider timeProvider,
    ILogger<TokenService> logger)
    : ITokenService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public TokenResult GenerateJwtToken(long userId)
    {
        logger.LogInformation("Generating new access token for user with id: {id}.", userId);

        var expiry = timeProvider.GetUtcNow().Add(_jwtOptions.AccessTokenLifeTime);
        var token = JwtBearer.CreateToken(opt =>
        {
            opt.SigningKey = _jwtOptions.SigningKey;
            opt.Issuer = _jwtOptions.Issuer;
            opt.Audience = _jwtOptions.Audience;
            opt.ExpireAt = expiry.UtcDateTime;
            opt.User.Claims.Add(new Claim(AuthenticationConstants.UserIdClaim, userId.ToString()));
        });

        return new TokenResult(token, expiry);
    }

    public TokenResult GenerateRefreshToken()
    {
        logger.LogInformation("Generating new refresh token.");

        var expiry = timeProvider.GetUtcNow().Add(_jwtOptions.RefreshTokenLifeTime);
        var randomNumber = new byte[64];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(randomNumber);

        return new TokenResult(Convert.ToBase64String(randomNumber), expiry);
    }

    public async Task<TokenResult?> RefreshAccessToken(string accessToken, string refreshToken,
        CancellationToken ct = default)
    {
        logger.LogInformation("Refreshing access token.");

        var tokenValidation = await ValidateAccessToken(accessToken);
        if (!tokenValidation.IsValid)
        {
            return null;
        }

        var id = Convert.ToInt64(tokenValidation.Claims[AuthenticationConstants.UserIdClaim]);
        var user = await unitOfWork.UserRepository.Get(id, ct);
        if (user is null || user.RefreshToken != refreshToken ||
            user.RefreshTokenExpiry!.Value < timeProvider.GetUtcNow())
        {
            return null;
        }

        return GenerateJwtToken(user.Id);
    }

    private Task<TokenValidationResult> ValidateAccessToken(string? token)
    {
        var validationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey)),
            ValidIssuer = _jwtOptions.Issuer,
            ValidAudience = _jwtOptions.Audience,
            ValidateLifetime = false
        };
        return new JsonWebTokenHandler().ValidateTokenAsync(token, validationParameters);
    }
}