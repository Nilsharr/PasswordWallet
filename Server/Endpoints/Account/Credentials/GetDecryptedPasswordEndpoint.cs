using PasswordWallet.Server.Services;
using PasswordWallet.Server.Utils;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.Server.Endpoints.Account.Credentials;

public class GetDecryptedPasswordEndpoint : Endpoint<IdRequestDto, string>
{
    private readonly ICredentialsService _credentialsService;

    public GetDecryptedPasswordEndpoint(ICredentialsService credentialsService)
    {
        _credentialsService = credentialsService;
    }

    public override void Configure()
    {
        Get("/api/account/credentials/{id:int}/decrypt");
        Claims(Constants.AccountIdClaim);
        Description(x => x.WithName("GetDecryptedPassword"));
    }

    public override async Task HandleAsync(IdRequestDto req, CancellationToken ct)
    {
        if (req.AccountId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var password = await _credentialsService.DecryptPassword(req.Id);
        await SendAsync(password, cancellation: ct);
    }
}