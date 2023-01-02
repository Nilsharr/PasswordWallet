using System.Net;
using PasswordWallet.Server.Entities;
using PasswordWallet.Server.Enums;
using PasswordWallet.Server.Repositories;
using PasswordWallet.Server.Utils;

namespace PasswordWallet.Server.Services;

public interface ILoginAuditService
{
    Task<(LoginIpAddress loginIpAddress, Account? account)> RegisterLoginAttempt(IPAddress ipAddress, string login,
        bool loginSuccessful,
        CancellationToken ct = default);

    Task<(bool permanentlyLocked, DateTime? temporaryLock)> GetIpAddressBlockadeStatus(IPAddress ipAddress,
        CancellationToken ct = default);

    Task<DateTime?> GetAccountBlockadeStatus(string login, CancellationToken ct = default);
}

public class LoginAuditService : ILoginAuditService
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAccountRepository _accountRepository;
    private readonly ILoginIpAddressRepository _loginIpAddressRepository;

    public LoginAuditService(IDateTimeProvider dateTimeProvider, IAccountRepository accountRepository,
        ILoginIpAddressRepository loginIpAddressRepository)
    {
        _dateTimeProvider = dateTimeProvider;
        _accountRepository = accountRepository;
        _loginIpAddressRepository = loginIpAddressRepository;
    }

    public async Task<(LoginIpAddress loginIpAddress, Account? account)> RegisterLoginAttempt(IPAddress ipAddress,
        string login, bool loginSuccessful, CancellationToken ct = default)
    {
        var account = await _accountRepository.Get(login, ct);
        var loginIpAddress = await _loginIpAddressRepository.Get(ipAddress, ct);

        if (account is not null)
        {
            var accountLogin = new AccountLogin {Correct = loginSuccessful, AccountId = account.Id};
            // first try to login to existing account from ip
            if (loginIpAddress is null)
            {
                loginIpAddress = new LoginIpAddress
                {
                    IpAddress = ipAddress,
                    AmountOfGoodLogins = loginSuccessful ? 1 : 0,
                    AmountOfBadLogins = loginSuccessful ? 0 : 1,
                    SubsequentBadLogins = loginSuccessful ? 0 : 1,
                    AccountLogins = new List<AccountLogin> {accountLogin}
                };
                _loginIpAddressRepository.Add(loginIpAddress);
            }
            // another try to login to existing account from ip
            else
            {
                loginIpAddress.AmountOfGoodLogins += loginSuccessful ? 1 : 0;
                loginIpAddress.AmountOfBadLogins += loginSuccessful ? 0 : 1;
                loginIpAddress.SubsequentBadLogins = loginSuccessful ? 0 : ++loginIpAddress.SubsequentBadLogins;
                loginIpAddress.AccountLogins.Add(accountLogin);
            }

            SetIpAddressLockout(loginIpAddress);

            account.SubsequentBadLogins = loginSuccessful ? 0 : ++account.SubsequentBadLogins;
            account.LockoutTime = loginSuccessful ? null : SetAccountLockout(account.SubsequentBadLogins);
        }
        // another login to nonexistent account from ip
        else if (loginIpAddress is not null)
        {
            loginIpAddress.AmountOfBadLogins++;
            loginIpAddress.SubsequentBadLogins++;

            SetIpAddressLockout(loginIpAddress);
        }
        // login to nonexistent account from new ip
        else
        {
            loginIpAddress = new LoginIpAddress {IpAddress = ipAddress, AmountOfBadLogins = 1, SubsequentBadLogins = 1};
            _loginIpAddressRepository.Add(loginIpAddress);
        }

        await _loginIpAddressRepository.SaveChanges(ct);
        return (loginIpAddress, account);
    }

    public async Task<(bool permanentlyLocked, DateTime? temporaryLock)> GetIpAddressBlockadeStatus(
        IPAddress ipAddress, CancellationToken ct = default)
    {
        var loginIpAddress = await _loginIpAddressRepository.Get(ipAddress, ct);
        return loginIpAddress is null ? (false, null) : (loginIpAddress.PermanentLock, loginIpAddress.TemporaryLock);
    }

    public async Task<DateTime?> GetAccountBlockadeStatus(string login, CancellationToken ct = default)
    {
        var account = await _accountRepository.Get(login, ct);
        return account?.LockoutTime;
    }

    private DateTime? SetAccountLockout(int incorrectLogins) => incorrectLogins switch
    {
        >= 0 and < (int) IncorrectAccountLoginsThreshold.Low => null,
        >= (int) IncorrectAccountLoginsThreshold.Low and < (int) IncorrectAccountLoginsThreshold.Medium =>
            _dateTimeProvider.UtcNow
                .AddSeconds(IncorrectAccountLoginsThreshold.Low.GetLockoutTime()),
        >= (int) IncorrectAccountLoginsThreshold.Medium and < (int) IncorrectAccountLoginsThreshold.High =>
            _dateTimeProvider.UtcNow
                .AddSeconds(IncorrectAccountLoginsThreshold.Medium.GetLockoutTime()),
        >= (int) IncorrectAccountLoginsThreshold.High => _dateTimeProvider.UtcNow.AddSeconds(
            IncorrectAccountLoginsThreshold.High.GetLockoutTime()),
        _ => throw new ArgumentException("Number cannot be negative", nameof(incorrectLogins))
    };

    private void SetIpAddressLockout(LoginIpAddress loginIpAddress)
    {
        if (loginIpAddress.SubsequentBadLogins >= (int) IncorrectIpAddressLoginsThreshold.High)
        {
            loginIpAddress.PermanentLock = true;
        }
        else
        {
            loginIpAddress.TemporaryLock = SetIpLockoutTime(loginIpAddress.SubsequentBadLogins);
        }
    }

    private DateTime? SetIpLockoutTime(int incorrectLogins) => incorrectLogins switch
    {
        >= 0 and < (int) IncorrectIpAddressLoginsThreshold.Low => null,
        >= (int) IncorrectIpAddressLoginsThreshold.Low and < (int) IncorrectIpAddressLoginsThreshold.Medium =>
            _dateTimeProvider.UtcNow
                .AddSeconds(IncorrectIpAddressLoginsThreshold.Low.GetLockoutTime()),
        >= (int) IncorrectIpAddressLoginsThreshold.Medium and < (int) IncorrectIpAddressLoginsThreshold.High =>
            _dateTimeProvider.UtcNow
                .AddSeconds((int) IncorrectIpAddressLoginsThreshold.Medium.GetLockoutTime()),
        >= (int) IncorrectIpAddressLoginsThreshold.High => throw new ArgumentException(
            "Cannot set time lockout when incorrect subsequent logins exceed " +
            (int) IncorrectIpAddressLoginsThreshold.High,
            nameof(incorrectLogins)),
        _ => throw new ArgumentException("Number cannot be negative", nameof(incorrectLogins))
    };
}