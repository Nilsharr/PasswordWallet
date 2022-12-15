using Microsoft.EntityFrameworkCore;
using PasswordWallet.Server.Data;
using PasswordWallet.Server.Entities;
using Z.EntityFramework.Plus;

namespace PasswordWallet.Server.Repositories;

public interface ICredentialRepository : IRepository<Credential>
{
    Task<IReadOnlyList<Credential>> GetAll(long accountId, CancellationToken ct = default);
    Task<Credential?> GetWithAccount(long credentialId, CancellationToken ct = default);
    Task<string?> GetPassword(long credentialId, CancellationToken ct = default);
    Task SaveChanges(CancellationToken ct = default);
}

public class CredentialRepository : ICredentialRepository
{
    private readonly PasswordWalletDbContext _dbContext;

    public CredentialRepository(PasswordWalletDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Credential>> GetAll(long accountId, CancellationToken ct = default)
    {
        return await _dbContext.Credential.Where(x => x.AccountId == accountId).OrderBy(x => x.Id).ToListAsync(ct);
    }

    public async Task<Credential?> Get(long credentialId, CancellationToken ct = default)
    {
        return await _dbContext.Credential.FindAsync(new object?[] {credentialId}, cancellationToken: ct);
    }

    public async Task<string?> GetPassword(long credentialId, CancellationToken ct = default)
    {
        return await _dbContext.Credential.Where(x => x.Id == credentialId).Select(x => x.Password)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<Credential?> GetWithAccount(long credentialId, CancellationToken ct = default)
    {
        return await _dbContext.Credential.Where(x => x.Id == credentialId).Include(x => x.Account)
            .SingleOrDefaultAsync(ct);
    }

    public Credential Add(Credential credential)
    {
        return _dbContext.Credential.Add(credential).Entity;
    }

    public Credential Update(Credential credential)
    {
        return _dbContext.Credential.Update(credential).Entity;
    }

    public async Task Delete(long credentialId, CancellationToken ct = default)
    {
        await _dbContext.Credential.Where(x => x.Id == credentialId).DeleteAsync(ct);
    }

    public async Task SaveChanges(CancellationToken ct = default)
    {
        await _dbContext.SaveChangesAsync(ct);
    }
}