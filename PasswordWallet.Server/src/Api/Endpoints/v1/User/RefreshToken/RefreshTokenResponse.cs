namespace Api.Endpoints.v1.User.RefreshToken;

public record RefreshTokenResponse(string AccessToken, DateTimeOffset AccessTokenExpiry);