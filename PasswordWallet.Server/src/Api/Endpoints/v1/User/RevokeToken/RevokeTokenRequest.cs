using FastEndpoints;

namespace Api.Endpoints.v1.User.RevokeToken;

public record RevokeTokenRequest([property: FromClaim] long UserId = 0);