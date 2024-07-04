using Core.Entities;

namespace Core.Interfaces.Services;

public interface ICredentialService
{
    Task<Credential> EncryptAndSaveCredential(long userId, Credential credential, CancellationToken ct = default);

    Task<Credential> EncryptAndUpdateCredential(long userId, Credential credential, CancellationToken ct = default);

    Task<string?> DecryptCredentialPassword(long userId, long credentialId, CancellationToken ct = default);

    Task UpdateCredentialsEncryption(long userId, string oldPassword, string newPassword,
        CancellationToken ct = default);
}