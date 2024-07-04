namespace Api.Endpoints.v1.User.RefreshToken;

public record RefreshTokenRequest(string AccessToken, string RefreshToken);