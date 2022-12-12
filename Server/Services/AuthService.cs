using FastEndpoints.Security;
using Microsoft.Extensions.Options;
using PasswordWallet.Server.Entities;
using PasswordWallet.Server.Repositories;
using PasswordWallet.Server.Utils;
using PasswordWallet.Shared.Dtos;
using Constants = PasswordWallet.Server.Utils.Constants;

namespace PasswordWallet.Server.Services;

public interface IAuthService
{
    string GenerateJwtToken(long accountId);
    Task<(bool valid, long? accountId)> AreCredentialsValid(LoginRequestDto req, CancellationToken ct = default);
    Task ChangePassword(long accountId, string newPassword, bool isPasswordKeptAsHash, CancellationToken ct = default);
}

public class AuthService : IAuthService
{
    private readonly AppSettings _appSettings;
    private readonly IAccountRepository _accountRepository;
    private readonly ICryptoService _cryptoService;

    public AuthService(IOptions<AppSettings> appSettings, IAccountRepository accountRepository,
        ICryptoService cryptoService)
    {
        _appSettings = appSettings.Value;
        _accountRepository = accountRepository;
        _cryptoService = cryptoService;
    }

    public string GenerateJwtToken(long accountId)
    {
        return JWTBearer.CreateToken(
            signingKey: _appSettings.JwtSigningKey,
            expireAt: DateTime.UtcNow.AddDays(1),
            claims: new[] {(Constants.AccountIdClaim, accountId.ToString())}
        );
    }

    public async Task<(bool valid, long? accountId)> AreCredentialsValid(LoginRequestDto req,
        CancellationToken ct = default)
    {
        var account = await _accountRepository.Get(req.Login, ct);
        if (account is null)
        {
            return (false, null);
        }

        if (account.IsPasswordKeptAsHash)
        {
            return (account.PasswordHash == _cryptoService.HashSha512(req.Password, account.Salt).hashedPassword,
                account.Id);
        }

        return (
            account.PasswordHash == _cryptoService.HashHmac(req.Password, _cryptoService.HexStringToBytes(account.Salt))
                .hashedPassword, account.Id);
    }

    public async Task ChangePassword(long accountId, string newPassword, bool isPasswordKeptAsHash,
        CancellationToken ct = default)
    {
        var account = await _accountRepository.GetWithCredentials(accountId, ct);
        var oldPasswordHash = account!.PasswordHash;

        var hash = isPasswordKeptAsHash ? _cryptoService.HashSha512(newPassword) : _cryptoService.HashHmac(newPassword);
        account.PasswordHash = hash.hashedPassword;
        account.Salt = hash.salt;
        account.IsPasswordKeptAsHash = isPasswordKeptAsHash;
        _accountRepository.Update(account);

        UpdateCredentialsEncryption(account, oldPasswordHash, ct);
        await _accountRepository.SaveChanges(ct);
    }

    private void UpdateCredentialsEncryption(Account account, string oldPassword, CancellationToken ct = default)
    {
        foreach (var credential in account.Credentials)
        {
            var pass = _cryptoService.AesDecryptToString(credential.Password, oldPassword);
            credential.Password = _cryptoService.AesEncryptToHexString(pass, account.PasswordHash);
        }
    }
}