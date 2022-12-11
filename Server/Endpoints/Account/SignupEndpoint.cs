using PasswordWallet.Server.Repositories;
using PasswordWallet.Server.Services;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.Server.Endpoints.Account;

public class SignupEndpoint : Endpoint<SignupRequestDto>
{
    private readonly ICryptoService _cryptoService;
    private readonly IAccountRepository _accountRepository;

    public SignupEndpoint(ICryptoService cryptoService, IAccountRepository accountRepository)
    {
        _cryptoService = cryptoService;
        _accountRepository = accountRepository;
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
        //TODO: Add logging
        if (await _accountRepository.Exists(req.Login, ct))
        {
            ThrowError("Login is already taken");
        }

        var hash = req.IsPasswordKeptAsHash
            ? _cryptoService.HashSha512(req.Password)
            : _cryptoService.HashHmac(req.Password);

        var account = new Entities.Account
        {
            Login = req.Login,
            PasswordHash = hash.hashedPassword,
            IsPasswordKeptAsHash = req.IsPasswordKeptAsHash,
            Salt = hash.salt
        };
        _accountRepository.Add(account);
        await _accountRepository.SaveChanges(ct);
        await SendOkAsync(ct);
    }
}