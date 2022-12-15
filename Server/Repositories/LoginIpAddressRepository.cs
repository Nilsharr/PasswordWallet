using System.Net;
using Microsoft.EntityFrameworkCore;
using PasswordWallet.Server.Data;
using PasswordWallet.Server.Entities;
using Z.EntityFramework.Plus;

namespace PasswordWallet.Server.Repositories;

public interface ILoginIpAddressRepository : IRepository<LoginIpAddress>
{
    Task<LoginIpAddress?> Get(IPAddress ipAddress, CancellationToken ct = default);
    Task SaveChanges(CancellationToken ct = default);
}

public class LoginIpAddressRepository : ILoginIpAddressRepository
{
    private readonly PasswordWalletDbContext _dbContext;

    public LoginIpAddressRepository(PasswordWalletDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LoginIpAddress?> Get(long id, CancellationToken ct = default)
    {
        return await _dbContext.LoginIpAddress.FindAsync(new object?[] {id}, cancellationToken: ct);
    }

    public async Task<LoginIpAddress?> Get(IPAddress ipAddress, CancellationToken ct = default)
    {
        return await _dbContext.LoginIpAddress.Where(x => x.IpAddress == ipAddress).FirstOrDefaultAsync(ct);
    }

    public LoginIpAddress Add(LoginIpAddress entity)
    {
        return _dbContext.LoginIpAddress.Add(entity).Entity;
    }

    public LoginIpAddress Update(LoginIpAddress entity)
    {
        return _dbContext.LoginIpAddress.Update(entity).Entity;
    }

    public async Task Delete(long id, CancellationToken ct = default)
    {
        await _dbContext.LoginIpAddress.Where(x => x.Id == id).DeleteAsync(ct);
    }

    public async Task SaveChanges(CancellationToken ct = default)
    {
        await _dbContext.SaveChangesAsync(ct);
    }
}