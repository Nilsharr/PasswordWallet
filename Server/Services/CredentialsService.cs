using Microsoft.EntityFrameworkCore;
using PasswordWallet.Server.Data;
using PasswordWallet.Shared.Dtos;
using PasswordWallet.Server.Entities;
using PasswordWallet.Server.Utils;
using Z.EntityFramework.Plus;

namespace PasswordWallet.Server.Services;

public interface ICredentialsService
{
    Task<IList<CredentialsDto>> GetCredentials(int accountId, CancellationToken ct = default);
    Task<CredentialsDto> GetCredential(int accountId, int credentialId, CancellationToken ct = default);
    Task<CredentialsDto> AddCredential(int accountId, CredentialsDto credentialDto, CancellationToken ct = default);
    Task<CredentialsDto> UpdateCredential(int accountId, CredentialsDto credentialDto, CancellationToken ct = default);
    Task DeleteCredential(int accountId, int credentialId, CancellationToken ct = default);
    Task UpdateCredentialsEncryption(int accountId, string newPassword, CancellationToken ct = default);
}

public class CredentialsService : ICredentialsService
{
    private readonly PasswordWalletDbContext _dbContext;
    private readonly IAccountService _accountService;
    private readonly AutoMapper.IMapper _mapper;

    public CredentialsService(PasswordWalletDbContext dbContext, IAccountService accountService,
        AutoMapper.IMapper mapper)
    {
        _dbContext = dbContext;
        _accountService = accountService;
        _mapper = mapper;
    }

    public async Task<IList<CredentialsDto>> GetCredentials(int accountId, CancellationToken ct = default)
    {
        var credentials = await GetCredentialsEntity(accountId, ct);
        return _mapper.Map<IList<CredentialsDto>>(credentials);
    }

    public async Task<CredentialsDto> GetCredential(int accountId, int credentialId, CancellationToken ct = default)
    {
        return _mapper.Map<CredentialsDto>(await _dbContext.Credentials
            .Where(x => x.AccountId == accountId && x.Id == credentialId).SingleOrDefaultAsync(ct));
    }

    // hmm
    public async Task<CredentialsDto> AddCredential(int accountId, CredentialsDto credentialDto,
        CancellationToken ct = default)
    {
        var credential = await ProcessAddUpdate(accountId, credentialDto, ct);
        await _dbContext.AddAsync(credential, ct);
        await _dbContext.SaveChangesAsync(ct);
        return _mapper.Map<CredentialsDto>(credential);
    }

    public async Task<CredentialsDto> UpdateCredential(int accountId, CredentialsDto credentialDto,
        CancellationToken ct = default)
    {
        var credential = await ProcessAddUpdate(accountId, credentialDto, ct);
        _dbContext.Update(credential);
        await _dbContext.SaveChangesAsync(ct);
        return _mapper.Map<CredentialsDto>(credential);
    }

    public async Task DeleteCredential(int accountId, int credentialId, CancellationToken ct = default)
    {
        await _dbContext.Credentials.Where(x => x.AccountId == accountId && x.Id == credentialId).DeleteAsync(ct);
    }

    private async Task<Credentials> ProcessAddUpdate(int accountId, CredentialsDto credentialDto, CancellationToken ct)
    {
        var credential = _mapper.Map<Credentials>(credentialDto);
        var account = await _accountService.GetAccount(accountId, ct);
        credential.AccountId = accountId;
        credential.Password = CryptoUtils.AesEncryptToHexString(credential.Password, account!.PasswordHash);
        return credential;
    }

    public async Task UpdateCredentialsEncryption(int accountId, string newPassword, CancellationToken ct = default)
    {
        var account = await _accountService.GetAccount(accountId, ct);
        var credentials = await GetCredentialsEntity(accountId, ct);
        foreach (var credential in credentials)
        {
            var pass = CryptoUtils.AesDecryptToString(credential.Password, account!.PasswordHash);
            credential.Password = CryptoUtils.AesEncryptToHexString(pass, newPassword);
        }

        await _dbContext.SaveChangesAsync(ct);
    }

    private async Task<IList<Credentials>> GetCredentialsEntity(int accountId, CancellationToken ct = default)
    {
        return await _dbContext.Credentials.Where(x => x.AccountId == accountId).OrderBy(x => x.Id)
            .ToListAsync(ct);
    }
}