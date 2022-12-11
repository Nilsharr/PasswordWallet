using PasswordWallet.Server.Services;
using PasswordWallet.Shared.Dtos;
using PasswordWallet.Server.Utils;

namespace PasswordWallet.Server.Endpoints.Account;

public class ChangePasswordEndpoint : Endpoint<ChangePasswordRequestDto>
{
    private readonly IAuthService _authService;

    public ChangePasswordEndpoint(IAuthService authService)
    {
        _authService = authService;
    }

    public override void Configure()
    {
        Patch("/api/account/change-password");
        Claims(Constants.AccountIdClaim);
        Description(x => x.WithName("ChangePassword").Produces(200));
    }

    public override async Task HandleAsync(ChangePasswordRequestDto req, CancellationToken ct)
    {
        if (req.AccountId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await _authService.ChangePassword(req.AccountId.Value, req.Password, req.IsPasswordKeptAsHash, ct);
        await SendOkAsync(ct);
    }
}