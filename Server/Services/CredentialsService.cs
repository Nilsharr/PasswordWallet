using PasswordWallet.Server.Entities;
using PasswordWallet.Server.Repositories;

namespace PasswordWallet.Server.Services;

public interface ICredentialsService
{
    Task<string> DecryptPassword(long accountId, long credentialId);
    Task<Credentials> EncryptAndSaveCredential(long accountId, Credentials credential, CancellationToken ct = default);

    Task<Credentials> EncryptAndUpdateCredential(long accountId, Credentials credential,
        CancellationToken ct = default);
}

public class CredentialsService : ICredentialsService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICredentialsRepository _credentialsRepository;
    private readonly ICryptoService _cryptoService;

    public CredentialsService(IAccountRepository accountRepository, ICredentialsRepository credentialsRepository,
        ICryptoService cryptoService)
    {
        _accountRepository = accountRepository;
        _credentialsRepository = credentialsRepository;
        _cryptoService = cryptoService;
    }

    public async Task<string> DecryptPassword(long accountId, long credentialId)
    {
        var credential = await _credentialsRepository.GetWithAccount(credentialId);
        return _cryptoService.AesDecryptToString(credential!.Password, credential.Account.PasswordHash);
    }

    public async Task<Credentials> EncryptAndSaveCredential(long accountId, Credentials credential,
        CancellationToken ct = default)
    {
        credential = await EncryptCredential(accountId, credential, ct);
        _credentialsRepository.Add(credential);
        await _credentialsRepository.SaveChanges(ct);
        return credential;
    }

    public async Task<Credentials> EncryptAndUpdateCredential(long accountId, Credentials credential,
        CancellationToken ct = default)
    {
        credential = await EncryptCredential(accountId, credential, ct);
        _credentialsRepository.Update(credential);
        await _credentialsRepository.SaveChanges(ct);
        return credential;
    }

    private async Task<Credentials> EncryptCredential(long accountId, Credentials credential,
        CancellationToken ct = default)
    {
        var account = await _accountRepository.Get(accountId, ct);
        credential.AccountId = accountId;
        credential.Password = _cryptoService.AesEncryptToHexString(credential.Password, account!.PasswordHash);
        return credential;
    }
}