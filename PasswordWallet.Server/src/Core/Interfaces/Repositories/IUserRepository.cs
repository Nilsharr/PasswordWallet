using Core.Entities;
using Core.Models;

namespace Core.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User, long>
{
    Task<User?> GetByUsername(string username, CancellationToken ct = default);
    Task<string?> GetPassword(long userId, CancellationToken ct = default);
    Task<DateTimeOffset?> GetLockoutTime(string username, CancellationToken ct = default);
    Task<bool> ExistsWithUsername(string username, CancellationToken ct = default);
    Task ExecuteUpdateRefreshToken(long userId, TokenResult refreshToken, CancellationToken ct = default);
    Task ExecuteRevokeRefreshToken(long userId, CancellationToken ct = default);
}