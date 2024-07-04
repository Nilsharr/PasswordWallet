using Core.Entities;
using Core.Models;

namespace Core.Interfaces.Services;

public interface IIdentityService
{
    Task<User> CreateNewUser(string username, string password, CancellationToken ct = default);
    Task<User?> Authenticate(string username, string providedPassword, CancellationToken ct = default);

    Task<AuthenticationResponse> CreateAuthenticationResponse(long userId, string username, bool withLastLoginDates,
        CancellationToken ct = default);

    Task<bool> ChangePassword(long userId, string currentPassword, string newPassword, CancellationToken ct = default);
}