using Microsoft.EntityFrameworkCore;
using PasswordWallet.Server.Data;
using PasswordWallet.Server.Entities;
using Z.EntityFramework.Plus;

namespace PasswordWallet.Server.Repositories;

public interface IAccountRepository : IRepository<Account>
{
    Task<Account?> Get(string login, CancellationToken ct = default);
    Task<Account?> GetWithCredentials(long accountId, CancellationToken ct = default);
    Task<bool> Exists(string login, CancellationToken ct = default);
    Task SaveChanges(CancellationToken ct = default);
}

public class AccountRepository : IAccountRepository
{
    private readonly PasswordWalletDbContext _dbContext;

    public AccountRepository(PasswordWalletDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Account?> Get(long accountId, CancellationToken ct = default)
    {
        return await _dbContext.Account.FindAsync(new object?[] {accountId}, cancellationToken: ct);
    }

    public async Task<Account?> Get(string login, CancellationToken ct = default)
    {
        return await _dbContext.Account.Where(x => x.Login == login).SingleOrDefaultAsync(ct);
    }

    public async Task<Account?> GetWithCredentials(long accountId, CancellationToken ct = default)
    {
        return await _dbContext.Account.Where(x => x.Id == accountId).Include(x => x.Credentials)
            .SingleOrDefaultAsync(ct);
    }

    public Account Add(Account account)
    {
        return _dbContext.Account.Add(account).Entity;
    }

    public Account Update(Account account)
    {
        return _dbContext.Account.Update(account).Entity;
    }

    public async Task Delete(long accountId, CancellationToken ct = default)
    {
        await _dbContext.Account.Where(x => x.Id == accountId).DeleteAsync(ct);
    }

    public async Task<bool> Exists(string login, CancellationToken ct = default)
    {
        return await _dbContext.Account.AnyAsync(x => x.Login == login, ct);
    }

    public async Task SaveChanges(CancellationToken ct = default)
    {
        await _dbContext.SaveChangesAsync(ct);
    }
}