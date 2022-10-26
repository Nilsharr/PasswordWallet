using System.Security.Cryptography;
using System.Text;
using FastEndpoints.Security;
using Microsoft.Extensions.Options;
using PasswordWallet.Server.Utils;
using PasswordWallet.Shared.Dtos;
using Constants = PasswordWallet.Server.Utils.Constants;

namespace PasswordWallet.Server.Services;

public interface IAuthService
{
    string GenerateJwtToken(int accountId);
    Task<string> DecryptPassword(int accountId, int credentialId);
    Task<(bool valid, int? accountId)> CredentialsAreValid(LoginRequestDto req, CancellationToken ct = default);
    (string hashedPassword, string salt) HashPassword(string password, bool isPasswordKeptAsHash);
    Task ChangePassword(int accountId, string newPassword, bool isPasswordKeptAsHash, CancellationToken ct = default);
}

public class AuthService : IAuthService
{
    private readonly AppSettings _appSettings;
    private readonly IAccountService _accountService;
    private readonly ICredentialsService _credentialsService;
    private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();

    public AuthService(IOptions<AppSettings> appSettings, IAccountService accountService,
        ICredentialsService credentialsService)
    {
        _appSettings = appSettings.Value;
        _accountService = accountService;
        _credentialsService = credentialsService;
    }

    public string GenerateJwtToken(int accountId)
    {
        return JWTBearer.CreateToken(
            signingKey: _appSettings.JwtSigningKey,
            expireAt: DateTime.UtcNow.AddDays(1),
            claims: new[] {(Constants.AccountIdClaim, accountId.ToString())}
        );
    }

    public async Task<string> DecryptPassword(int accountId, int credentialId)
    {
        var account = await _accountService.GetAccount(accountId);
        var credential = await _credentialsService.GetCredential(accountId, credentialId);
        return AesEncryptor.DecryptToString(credential.Password, account!.PasswordHash);
    }

    public async Task<(bool valid, int? accountId)> CredentialsAreValid(LoginRequestDto req,
        CancellationToken ct = default)
    {
        var account = await _accountService.GetAccount(req.Login, ct);
        if (account is null)
        {
            return (false, null);
        }

        if (account.IsPasswordKeptAsHash)
        {
            return (account.PasswordHash == HashPasswordSha512(req.Password, account.Salt).hashedPassword, account.Id);
        }

        //TODO move HexStringToBytes to utils class? (and GenerateSalt?)
        return (
            account.PasswordHash == HashPasswordHmac(req.Password, AesEncryptor.HexStringToBytes(account.Salt!))
                .hashedPassword, account.Id);
    }

    public (string hashedPassword, string salt) HashPassword(string password, bool isPasswordKeptAsHash)
    {
        return isPasswordKeptAsHash ? HashPasswordSha512(password) : HashPasswordHmac(password);
    }

    public async Task ChangePassword(int accountId, string newPassword, bool isPasswordKeptAsHash,
        CancellationToken ct = default)
    {
        var hash = HashPassword(newPassword, isPasswordKeptAsHash);
        await _accountService.UpdatePassword(accountId, hash, isPasswordKeptAsHash, ct);
    }

    private (string hashedPassword, string salt) HashPasswordSha512(string password, string? salt = null)
    {
        salt ??= GenerateSalt(64);
        password = _appSettings.PasswordPepper + salt + password;
        var bytes = Encoding.UTF8.GetBytes(password);
        var sha512 = SHA512.HashData(bytes);
        return (Convert.ToHexString(sha512), salt);
    }

    private static (string hashedPassword, string salt) HashPasswordHmac(string password, byte[]? key = null)
    {
        using var hmac = key is null ? new HMACSHA512() : new HMACSHA512(key);
        key = hmac.Key;
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return (Convert.ToHexString(hash), Convert.ToHexString(key));
    }

    private static string GenerateSalt(int saltLength)
    {
        var salt = new byte[saltLength];
        Random.GetBytes(salt);
        return Convert.ToHexString(salt);
    }
}