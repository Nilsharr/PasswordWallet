using Api.Config.Groups;
using Core.Interfaces.Services;
using Core.Models;
using FastEndpoints;

namespace Api.Endpoints.v1.User.Login;

public class LoginEndpoint(IIdentityService identityService) : Endpoint<LoginRequest, AuthenticationResponse>
{
    public override void Configure()
    {
        Post("/login");
        AllowAnonymous();
        PreProcessors(new PreLoginProcessor<LoginRequest>());
        PostProcessors(new PostLoginProcessor<LoginRequest, AuthenticationResponse>());
        Group<UserGroup>();
        Summary(s => s.ExampleRequest = new LoginRequest("TestUser", "t#e#7Hyb^k"));
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await identityService.Authenticate(req.Username, req.Password, ct);
        if (user is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var response = await identityService.CreateAuthenticationResponse(user.Id, user.Username, true, ct);
        await SendOkAsync(response, ct);
    }
}