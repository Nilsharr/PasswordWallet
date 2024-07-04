using Api.Config.Groups;
using Core.Interfaces.Services;
using FastEndpoints;

namespace Api.Endpoints.v1.User.RefreshToken;

public class RefreshTokenEndpoint(ITokenService tokenService) : Endpoint<RefreshTokenRequest, RefreshTokenResponse>
{
    public override void Configure()
    {
        Post("/refresh");
        AllowAnonymous();
        Group<UserGroup>();
        Description(x => x.Produces(401));
    }

    public override async Task HandleAsync(RefreshTokenRequest req, CancellationToken ct)
    {
        var result = await tokenService.RefreshAccessToken(req.AccessToken, req.RefreshToken, ct);

        if (result is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var response = new RefreshTokenResponse(result.Token, result.Expiry);
        await SendOkAsync(response, ct);
    }
}