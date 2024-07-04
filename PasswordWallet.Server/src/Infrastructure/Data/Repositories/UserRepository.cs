using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class UserRepository(PasswordWalletDbContext dbContext)
    : GenericRepository<User, long>(dbContext), IUserRepository
{
    private readonly PasswordWalletDbContext _dbContext = dbContext;

    public Task<User?> GetByUsername(string username, CancellationToken ct = default)
    {
        return _dbContext.Users.Where(x => x.Username == username).SingleOrDefaultAsync(ct);
    }

    public Task<string?> GetPassword(long userId, CancellationToken ct = default)
    {
        return _dbContext.Users.Where(x => x.Id == userId).Select(x => x.PasswordHash).SingleOrDefaultAsync(ct);
    }

    public Task<DateTimeOffset?> GetLockoutTime(string username, CancellationToken ct = default)
    {
        return _dbContext.Users.Where(x => x.Username == username).Select(x => x.LockoutTime).SingleOrDefaultAsync(ct);
    }

    public Task<bool> ExistsWithUsername(string username, CancellationToken ct = default)
    {
        return _dbContext.Users.AnyAsync(x => x.Username == username, ct);
    }

    public Task ExecuteUpdateRefreshToken(long userId, TokenResult refreshToken,
        CancellationToken ct = default)
    {
        return _dbContext.Users.Where(x => x.Id == userId).ExecuteUpdateAsync(
            x => x.SetProperty(u => u.RefreshToken, refreshToken.Token)
                .SetProperty(u => u.RefreshTokenExpiry, refreshToken.Expiry), ct);
    }

    public Task ExecuteRevokeRefreshToken(long userId, CancellationToken ct = default)
    {
        return _dbContext.Users.Where(x => x.Id == userId).ExecuteUpdateAsync(
            x => x.SetProperty(u => u.RefreshToken, (string?)null)
                .SetProperty(u => u.RefreshTokenExpiry, (DateTimeOffset?)null), ct);
    }
}