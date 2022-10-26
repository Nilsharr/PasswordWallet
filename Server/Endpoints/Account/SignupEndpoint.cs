using PasswordWallet.Server.Services;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.Server.Endpoints.Account;

public class SignupEndpoint : Endpoint<SignupRequestDto>
{
    private readonly IAuthService _authService;
    private readonly IAccountService _accountService;

    public SignupEndpoint(IAuthService authService, IAccountService accountService)
    {
        _authService = authService;
        _accountService = accountService;
    }

    public override void Configure()
    {
        Post("/api/account/signup");
        AllowAnonymous();
        Description(x => x.WithName("Signup").Produces(200));
    }

    public override async Task HandleAsync(SignupRequestDto req, CancellationToken ct)
    {
        //TODO: Add this check to validator
        if (await _accountService.AccountExists(req.Login, ct))
        {
            ThrowError("Login is already taken");
        }

        var hash = _authService.HashPassword(req.Password, req.IsPasswordKeptAsHash);
        var account = new AccountDto
        {
            Login = req.Login,
            PasswordHash = hash.hashedPassword,
            IsPasswordKeptAsHash = req.IsPasswordKeptAsHash,
            Salt = hash.salt
        };
        await _accountService.AddAccount(account, ct);
        await SendOkAsync(ct);
    }
}