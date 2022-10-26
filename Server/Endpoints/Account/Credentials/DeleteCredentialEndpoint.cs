using System.Security.Claims;
using PasswordWallet.Server.Services;
using PasswordWallet.Shared.Dtos;
using PasswordWallet.Server.Utils;

namespace PasswordWallet.Server.Endpoints.Account.Credentials;

public class DeleteCredentialEndpoint : Endpoint<IdRequestDto>
{
    private readonly ICredentialsService _credentialsService;

    public DeleteCredentialEndpoint(ICredentialsService credentialsService)
    {
        _credentialsService = credentialsService;
    }

    public override void Configure()
    {
        Delete("/api/account/credentials/{id}");
        Claims(Constants.AccountIdClaim);
        Description(x => x.WithName("DeleteCredential").Produces(200));
    }

    public override async Task HandleAsync(IdRequestDto req, CancellationToken ct)
    {
        var accountId = JwtClaims.GetAccountIdFromClaims(HttpContext.User.Identity as ClaimsIdentity);
        if (accountId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await _credentialsService.DeleteCredential(accountId.Value, req.Id, ct);
        await SendOkAsync(ct);
    }
}