using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Core.Services;

public class CredentialService(
    IUnitOfWork unitOfWork,
    IEncryptionService encryptionService,
    ILogger<CredentialService> logger)
    : ICredentialService
{
    public async Task<Credential> EncryptAndSaveCredential(long userId, Credential credential,
        CancellationToken ct = default)
    {
        logger.LogInformation("Creating new credential for user with id: {userId}.", userId);

        if (!string.IsNullOrWhiteSpace(credential.Password))
        {
            var userPassword = (await unitOfWork.UserRepository.GetPassword(userId, ct))!;
            credential.Password = encryptionService.Encrypt(credential.Password, userPassword);
        }

        credential.Position = await unitOfWork.CredentialRepository.GetNextAvailablePosition(credential.FolderId, ct);

        await unitOfWork.CredentialRepository.Add(credential, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return credential;
    }

    public async Task<Credential> EncryptAndUpdateCredential(long userId, Credential credential,
        CancellationToken ct = default)
    {
        logger.LogInformation("Updating credential with id: {credentialId} for user with id: {userId}.", credential.Id,
            userId);

        var credentialToUpdate = (await unitOfWork.CredentialRepository.Get(credential.Id, ct))!;
        credentialToUpdate.Description = credential.Description;
        credentialToUpdate.Username = credential.Username;
        credentialToUpdate.WebAddress = credential.WebAddress;

        if (string.IsNullOrWhiteSpace(credential.Password))
        {
            credentialToUpdate.Password = credential.Password;
        }
        else
        {
            var userPassword = (await unitOfWork.UserRepository.GetPassword(userId, ct))!;
            credentialToUpdate.Password = encryptionService.Encrypt(credential.Password, userPassword);
        }

        await unitOfWork.SaveChangesAsync(ct);

        return credentialToUpdate;
    }

    public async Task<string?> DecryptCredentialPassword(long userId, long credentialId, CancellationToken ct = default)
    {
        logger.LogInformation("Decrypting credential password for user with id: {userId}.", userId);

        var credentialPassword = await unitOfWork.CredentialRepository.GetPassword(credentialId, ct);
        if (string.IsNullOrWhiteSpace(credentialPassword))
        {
            return credentialPassword;
        }

        var userPassword = (await unitOfWork.UserRepository.GetPassword(userId, ct))!;
        return encryptionService.Decrypt(credentialPassword, userPassword);
    }

    public async Task UpdateCredentialsEncryption(long userId, string oldPassword, string newPassword,
        CancellationToken ct = default)
    {
        logger.LogInformation("Updating credentials passwords key for user with id: {id}.", userId);

        var credentials = await unitOfWork.CredentialRepository.GetUserCredentials(userId, ct);
        foreach (var credential in credentials)
        {
            if (string.IsNullOrWhiteSpace(credential.Password))
            {
                continue;
            }

            var decrypted = encryptionService.Decrypt(credential.Password, oldPassword);
            credential.Password = encryptionService.Encrypt(decrypted, newPassword);
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}