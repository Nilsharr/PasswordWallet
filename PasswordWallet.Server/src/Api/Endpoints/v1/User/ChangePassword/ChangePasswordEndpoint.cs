using Api.Config.Groups;
using Core.Constants;
using Core.Interfaces.Services;
using FastEndpoints;

namespace Api.Endpoints.v1.User.ChangePassword;

public class ChangePasswordEndpoint(IIdentityService identityService) : Endpoint<ChangePasswordRequest>
{
    public override void Configure()
    {
        Patch("/change-password");
        Policies(AuthenticationConstants.UserPolicy);
        Group<UserGroup>();
        Description(x => x.ClearDefaultProduces(200).Produces(204));
        Summary(s => s.ExampleRequest = new ChangePasswordRequest("t#e#7Hyb^k","s4&jAZW!d63", "s4&jAZW!d63"));
    }

    public override async Task HandleAsync(ChangePasswordRequest req, CancellationToken ct)
    {
        var result = await identityService.ChangePassword(req.UserId, req.CurrentPassword, req.Password, ct);
        if (!result)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await SendNoContentAsync(ct);
    }
}