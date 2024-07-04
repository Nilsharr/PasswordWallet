namespace Core.Models;

public record AuthenticationResponse(
    string Username,
    DateTimeOffset? LastSuccessfulLogin,
    DateTimeOffset? LastUnsuccessfulLogin,
    string AccessToken,
    DateTimeOffset AccessTokenExpiry,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiry);