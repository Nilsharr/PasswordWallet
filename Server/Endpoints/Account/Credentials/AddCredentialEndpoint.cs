using System.Security.Claims;
using PasswordWallet.Server.Services;
using PasswordWallet.Shared.Dtos;
using PasswordWallet.Server.Utils;

namespace PasswordWallet.Server.Endpoints.Account.Credentials;

public class AddCredentialEndpoint : Endpoint<CredentialsDto, CredentialsDto>
{
    private readonly ICredentialsService _credentialsService;

    public AddCredentialEndpoint(ICredentialsService credentialsService)
    {
        _credentialsService = credentialsService;
    }

    public override void Configure()
    {
        Post("/api/account/credentials");
        Claims(Constants.AccountIdClaim);
        Description(x => x.WithName("AddCredential").Produces<CredentialsDto>(201));
    }

    public override async Task HandleAsync(CredentialsDto req, CancellationToken ct)
    {
        // TODO: clean this up so its not in every endpoint
        var accountId = JwtClaims.GetAccountIdFromClaims(HttpContext.User.Identity as ClaimsIdentity);
        if (accountId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var response = await _credentialsService.AddCredential(accountId.Value, req, ct);
        await SendCreatedAtAsync("GetCredential", new {response.Id}, response, cancellation: ct);
    }
}