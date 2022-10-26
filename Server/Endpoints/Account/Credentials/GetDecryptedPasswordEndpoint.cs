using System.Security.Claims;
using PasswordWallet.Server.Services;
using PasswordWallet.Server.Utils;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.Server.Endpoints.Account.Credentials;

public class GetDecryptedPasswordEndpoint : Endpoint<IdRequestDto, string>
{
    private readonly IAuthService _authService;

    public GetDecryptedPasswordEndpoint(IAuthService authService)
    {
        _authService = authService;
    }

    public override void Configure()
    {
        Get("/api/account/credentials/{id:int}/decrypt");
        Claims(Constants.AccountIdClaim);
        Description(x => x.WithName("GetDecryptedPassword"));
    }

    public override async Task HandleAsync(IdRequestDto req, CancellationToken ct)
    {
        var accountId = JwtClaims.GetAccountIdFromClaims(HttpContext.User.Identity as ClaimsIdentity);
        if (accountId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var password = await _authService.DecryptPassword(accountId.Value, req.Id);
        await SendAsync(password, cancellation: ct);
    }
}