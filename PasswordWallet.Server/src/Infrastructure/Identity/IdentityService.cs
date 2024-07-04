using Core.Constants;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Identity;

public class IdentityService(
    IUnitOfWork unitOfWork,
    IPasswordHasher<User> passwordHasher,
    ICredentialService credentialService,
    ITokenService tokenService,
    ILogger<IdentityService> logger) : IIdentityService
{
    public async Task<User> CreateNewUser(string username, string password,
        CancellationToken ct = default)
    {
        logger.LogInformation("Creating new user.");

        var user = new User
        {
            Username = username,
            Folders = new List<Folder>
            {
                new() { Name = FolderConstants.DefaultFolderName, Position = 1 }
            }
        };

        user.PasswordHash = passwordHasher.HashPassword(user, password);
        await unitOfWork.UserRepository.Add(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return user;
    }

    public async Task<User?> Authenticate(string username, string providedPassword, CancellationToken ct = default)
    {
        logger.LogInformation("Authenticating user with username: {username}.", username);

        var user = await unitOfWork.UserRepository.GetByUsername(username, ct);
        if (user is null)
        {
            return null;
        }

        return VerifyPassword(user, providedPassword) == PasswordVerificationResult.Failed ? null : user;
    }

    public async Task<AuthenticationResponse> CreateAuthenticationResponse(long userId, string username,
        bool withLastLoginDates, CancellationToken ct = default)
    {
        logger.LogInformation("Creating authentication response for user with id: {id}.", userId);

        var accessToken = tokenService.GenerateJwtToken(userId);
        var refreshToken = tokenService.GenerateRefreshToken();
        await unitOfWork.UserRepository.ExecuteUpdateRefreshToken(userId, refreshToken, ct);

        var loginDates = withLastLoginDates
            ? await unitOfWork.LoginHistoryRepository.GetLastLoginDates(userId, ct)
            : (null, null);

        return new AuthenticationResponse(username, loginDates.lastValid, loginDates.lastInvalid,
            accessToken.Token, accessToken.Expiry, refreshToken.Token, refreshToken.Expiry);
    }

    public async Task<bool> ChangePassword(long userId, string currentPassword, string newPassword,
        CancellationToken ct = default)
    {
        logger.LogInformation("Changing password for user with id: {id}.", userId);

        var user = (await unitOfWork.UserRepository.Get(userId, ct))!;
        if (VerifyPassword(user, currentPassword) == PasswordVerificationResult.Failed)
        {
            return false;
        }

        var oldPasswordHash = user.PasswordHash;
        var newPasswordHash = passwordHasher.HashPassword(user, newPassword);
        user.PasswordHash = newPasswordHash;
        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await unitOfWork.SaveChangesAsync(ct);

        await credentialService.UpdateCredentialsEncryption(userId, oldPasswordHash, newPasswordHash, ct);
        return true;
    }

    private PasswordVerificationResult VerifyPassword(User user, string providedPassword)
    {
        return passwordHasher.VerifyHashedPassword(user, user.PasswordHash, providedPassword);
    }
}