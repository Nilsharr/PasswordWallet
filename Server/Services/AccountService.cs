using Microsoft.EntityFrameworkCore;
using PasswordWallet.Server.Data;
using PasswordWallet.Server.Entities;
using PasswordWallet.Shared.Dtos;
using Z.EntityFramework.Plus;

namespace PasswordWallet.Server.Services;

public interface IAccountService
{
    Task<AccountDto?> GetAccount(int accountId, CancellationToken ct = default);
    Task<AccountDto?> GetAccount(string login, CancellationToken ct = default);
    Task<int> AddAccount(AccountDto accountDto, CancellationToken ct = default);

    Task UpdatePassword(int accountId, (string newPassword, string salt) hash, bool isPasswordKeptAsHash,
        CancellationToken ct = default);

    Task DeleteAccount(int accountId, CancellationToken ct = default);
    Task<bool> AccountExists(string login, CancellationToken ct = default);
}

public class AccountService : IAccountService
{
    private readonly PasswordWalletDbContext _dbContext;
    private readonly AutoMapper.IMapper _mapper;

    public AccountService(PasswordWalletDbContext dbContext, AutoMapper.IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<AccountDto?> GetAccount(int accountId, CancellationToken ct = default)
    {
        return _mapper.Map<AccountDto>(
            await _dbContext.Account.FindAsync(new object?[] {accountId}, cancellationToken: ct));
        //return _mapper.Map<AccountDto>(await _dbContext.Account.Where(x => x.Id == accountId).SingleOrDefaultAsync(ct));
    }

    public async Task<AccountDto?> GetAccount(string login, CancellationToken ct = default)
    {
        return _mapper.Map<AccountDto>(await _dbContext.Account.Where(x => x.Login == login).SingleOrDefaultAsync(ct));
    }

    public async Task<int> AddAccount(AccountDto accountDto, CancellationToken ct = default)
    {
        var acc = _mapper.Map<Account>(accountDto);
        await _dbContext.Account.AddAsync(acc, ct);
        await _dbContext.SaveChangesAsync(ct);
        return acc.Id;
    }

    public async Task UpdatePassword(int accountId, (string newPassword, string salt) hash, bool isPasswordKeptAsHash,
        CancellationToken ct = default)
    {
        //var account = await _dbContext.Account.Where(x => x.Id == accountId).SingleAsync(ct);
        var account = await _dbContext.Account.FindAsync(new object?[] {accountId}, cancellationToken: ct);
        account!.PasswordHash = hash.newPassword;
        account.Salt = hash.salt;
        account.IsPasswordKeptAsHash = isPasswordKeptAsHash;
        _dbContext.Update(account);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAccount(int accountId, CancellationToken ct = default)
    {
        await _dbContext.Account.Where(x => x.Id == accountId).DeleteAsync(ct);
    }

    public async Task<bool> AccountExists(string login, CancellationToken ct = default)
    {
        return await _dbContext.Account.AnyAsync(x => x.Login == login, ct);
    }
}