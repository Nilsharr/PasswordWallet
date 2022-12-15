using PasswordWallet.Server.Entities;
using PasswordWallet.Server.Repositories;

namespace PasswordWallet.Server.Services;

public interface ICredentialService
{
    Task<string> DecryptPassword(long credentialId);
    Task<Credential> EncryptAndSaveCredential(long accountId, Credential credential, CancellationToken ct = default);

    Task<Credential> EncryptAndUpdateCredential(long accountId, Credential credential,
        CancellationToken ct = default);
}

public class CredentialService : ICredentialService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICredentialRepository _credentialRepository;
    private readonly ICryptoService _cryptoService;

    public CredentialService(IAccountRepository accountRepository, ICredentialRepository credentialRepository,
        ICryptoService cryptoService)
    {
        _accountRepository = accountRepository;
        _credentialRepository = credentialRepository;
        _cryptoService = cryptoService;
    }

    public async Task<string> DecryptPassword(long credentialId)
    {
        var credential = await _credentialRepository.GetWithAccount(credentialId);
        return _cryptoService.AesDecryptToString(credential!.Password, credential.Account.PasswordHash);
    }

    public async Task<Credential> EncryptAndSaveCredential(long accountId, Credential credential,
        CancellationToken ct = default)
    {
        credential = await EncryptCredential(accountId, credential, ct);
        _credentialRepository.Add(credential);
        await _credentialRepository.SaveChanges(ct);
        return credential;
    }

    public async Task<Credential> EncryptAndUpdateCredential(long accountId, Credential credential,
        CancellationToken ct = default)
    {
        var currentPassword = await _credentialRepository.GetPassword(credential.Id, ct);
        if (credential.Password != currentPassword)
        {
            credential = await EncryptCredential(accountId, credential, ct);
        }

        _credentialRepository.Update(credential);
        await _credentialRepository.SaveChanges(ct);
        return credential;
    }

    private async Task<Credential> EncryptCredential(long accountId, Credential credential,
        CancellationToken ct = default)
    {
        var account = await _accountRepository.Get(accountId, ct);
        credential.Password = _cryptoService.AesEncryptToHexString(credential.Password, account!.PasswordHash);
        return credential;
    }
}