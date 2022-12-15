using PasswordWallet.Server.Repositories;
using PasswordWallet.Server.Services;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.Server.Endpoints.Account.Login;

public class LoginEndpoint : Endpoint<LoginRequestDto, AuthorizationResponseDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAuthService _authService;

    public LoginEndpoint(IAccountRepository accountRepository, IAuthService authService)
    {
        _accountRepository = accountRepository;
        _authService = authService;
    }

    public override void Configure()
    {
        Post("/api/account/login");
        PreProcessors(new PreLoginCheck<LoginRequestDto>());
        PostProcessors(new PostLoginLogger<LoginRequestDto, AuthorizationResponseDto>());
        AllowAnonymous();
        Description(x => x.WithName("Login"));
        //Description(x => x.WithName("Login").Produces<CredentialDto>(201));
    }


    public override async Task HandleAsync(LoginRequestDto req, CancellationToken ct)
    {
        var areCredentialsValid = await _authService.AreCredentialsValid(req, ct);
        if (areCredentialsValid.valid)
        {
            var jwtToken = _authService.GenerateJwtToken(areCredentialsValid.accountId!.Value);
            var lastLogins =
                await _accountRepository.GetLastValidAndInvalidLogin(areCredentialsValid.accountId.Value, ct);

            await SendAsync(
                new AuthorizationResponseDto
                {
                    Token = jwtToken, LastSuccessfulLogin = lastLogins.lastSuccessfulLogin,
                    LastUnsuccessfulLogin = lastLogins.lastUnsuccessfulLogin
                }, cancellation: ct);
        }
        else
        {
            await SendUnauthorizedAsync(ct);
        }
    }
}