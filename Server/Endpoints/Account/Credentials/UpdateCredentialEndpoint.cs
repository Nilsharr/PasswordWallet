using System.Security.Claims;
using PasswordWallet.Server.Services;
using PasswordWallet.Shared.Dtos;
using PasswordWallet.Server.Utils;

namespace PasswordWallet.Server.Endpoints.Account.Credentials;

public class UpdateCredentialEndpoint : Endpoint<CredentialsDto, CredentialsDto>
{
    private readonly ICredentialsService _credentialsService;

    public UpdateCredentialEndpoint(ICredentialsService credentialsService)
    {
        _credentialsService = credentialsService;
    }

    public override void Configure()
    {
        Put("/api/account/credentials/{id}");
        Claims(Constants.AccountIdClaim);
        Description(x => x.WithName("UpdateCredential"));
    }

    public override async Task HandleAsync(CredentialsDto req, CancellationToken ct)
    {
        var accountId = JwtClaims.GetAccountIdFromClaims(HttpContext.User.Identity as ClaimsIdentity);
        if (accountId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var response = await _credentialsService.UpdateCredential(accountId.Value, req, ct);
        await SendAsync(response, cancellation: ct);
    }
}