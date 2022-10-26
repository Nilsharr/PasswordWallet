using PasswordWallet.Server.Services;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.Server.Endpoints.Account;

public class LoginEndpoint : Endpoint<LoginRequestDto, AuthorizationResponseDto>
{
    private readonly IAuthService _authService;

    public LoginEndpoint(IAuthService authService)
    {
        _authService = authService;
    }

    public override void Configure()
    {
        Post("/api/account/login");
        AllowAnonymous();
        Description(x => x.WithName("Login"));
    }

    public override async Task HandleAsync(LoginRequestDto req, CancellationToken ct)
    {
        var areCredentialsValid = await _authService.CredentialsAreValid(req, ct);
        if (areCredentialsValid.valid)
        {
            var jwtToken = _authService.GenerateJwtToken(areCredentialsValid.accountId!.Value);
            await SendAsync(new AuthorizationResponseDto {Token = jwtToken}, cancellation: ct);
        }
        else
        {
            ThrowError("The supplied credentials are invalid");
        }
    }
}