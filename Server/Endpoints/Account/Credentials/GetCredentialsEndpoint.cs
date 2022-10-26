using System.Security.Claims;
using PasswordWallet.Server.Services;
using PasswordWallet.Server.Utils;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.Server.Endpoints.Account.Credentials;

// TODO: add pagination
public class GetCredentialsEndpoint : Endpoint<EmptyRequest, IList<CredentialsDto>>
{
    private readonly ICredentialsService _credentialsService;

    public GetCredentialsEndpoint(ICredentialsService credentialsService)
    {
        _credentialsService = credentialsService;
    }

    public override void Configure()
    {
        Get("/api/account/credentials");
        Claims(Constants.AccountIdClaim);
        Description(x => x.WithName("GetCredentials"));
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var accountId = JwtClaims.GetAccountIdFromClaims(HttpContext.User.Identity as ClaimsIdentity);
        if (accountId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var credentials = await _credentialsService.GetCredentials(accountId.Value, ct);
        await SendAsync(credentials, cancellation: ct);
    }
}