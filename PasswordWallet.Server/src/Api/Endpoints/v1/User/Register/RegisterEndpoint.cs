using Api.Config.Groups;
using Core.Interfaces.Services;
using Core.Models;
using FastEndpoints;

namespace Api.Endpoints.v1.User.Register;

public class RegisterEndpoint(IIdentityService identityService) : Endpoint<RegisterRequest, AuthenticationResponse>
{
    public override void Configure()
    {
        Post("/register");
        AllowAnonymous();
        Group<UserGroup>();
        Summary(s => s.ExampleRequest = new RegisterRequest("TestUser", "t#e#7Hyb^k", "t#e#7Hyb^k"));
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var user = await identityService.CreateNewUser(req.Username, req.Password, ct);
        var response = await identityService.CreateAuthenticationResponse(user.Id, user.Username, false, ct);
        await SendOkAsync(response, ct);
    }
}