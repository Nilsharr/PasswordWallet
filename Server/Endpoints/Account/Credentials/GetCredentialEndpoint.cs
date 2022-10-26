using System.Security.Claims;
using PasswordWallet.Server.Services;
using PasswordWallet.Server.Utils;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.Server.Endpoints.Account.Credentials;

public class GetCredentialEndpoint : Endpoint<IdRequestDto, CredentialsDto>
{
    private readonly ICredentialsService _credentialsService;

    public GetCredentialEndpoint(ICredentialsService credentialsService)
    {
        _credentialsService = credentialsService;
    }

    public override void Configure()
    {
        Get("/api/account/credentials/{id}");
        Claims(Constants.AccountIdClaim);
        Description(x => x.WithName("GetCredential"));
    }

    public override async Task HandleAsync(IdRequestDto req, CancellationToken ct)
    {
        var accountId = JwtClaims.GetAccountIdFromClaims(HttpContext.User.Identity as ClaimsIdentity);
        if (accountId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var credentials = await _credentialsService.GetCredential(accountId.Value, req.Id, ct);
        await SendAsync(credentials, cancellation: ct);
    }
}