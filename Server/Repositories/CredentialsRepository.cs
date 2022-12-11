using Microsoft.EntityFrameworkCore;
using PasswordWallet.Server.Data;
using PasswordWallet.Server.Entities;
using Z.EntityFramework.Plus;

namespace PasswordWallet.Server.Repositories;

public interface ICredentialsRepository : IRepository<Credentials>
{
    Task<IReadOnlyList<Credentials>> GetAll(long accountId, CancellationToken ct = default);
    Task<Credentials?> GetWithAccount(long credentialId, CancellationToken ct = default);
    Task SaveChanges(CancellationToken ct = default);
}

public class CredentialsRepository : ICredentialsRepository
{
    private readonly PasswordWalletDbContext _dbContext;

    public CredentialsRepository(PasswordWalletDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Credentials>> GetAll(long accountId, CancellationToken ct = default)
    {
        return await _dbContext.Credentials.Where(x => x.AccountId == accountId).OrderBy(x => x.Id).ToListAsync(ct);
    }

    public async Task<Credentials?> Get(long credentialId, CancellationToken ct = default)
    {
        return await _dbContext.Credentials.FindAsync(new object?[] {credentialId}, cancellationToken: ct);
    }

    public async Task<Credentials?> GetWithAccount(long credentialId, CancellationToken ct = default)
    {
        return await _dbContext.Credentials.Where(x => x.Id == credentialId).Include(x => x.Account)
            .SingleOrDefaultAsync(ct);
    }

    public Credentials Add(Credentials credential)
    {
        return _dbContext.Credentials.Add(credential).Entity;
    }

    public Credentials Update(Credentials credential)
    {
        return _dbContext.Credentials.Update(credential).Entity;
    }

    public async Task Delete(long credentialId, CancellationToken ct = default)
    {
        await _dbContext.Credentials.Where(x => x.Id == credentialId).DeleteAsync(ct);
    }

    public async Task SaveChanges(CancellationToken ct = default)
    {
        await _dbContext.SaveChangesAsync(ct);
    }
}