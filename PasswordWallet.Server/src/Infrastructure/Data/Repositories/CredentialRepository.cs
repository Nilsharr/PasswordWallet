using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class CredentialRepository(PasswordWalletDbContext dbContext)
    : GenericRepository<Credential, long>(dbContext), ICredentialRepository
{
    private readonly PasswordWalletDbContext _dbContext = dbContext;

    public async Task<PaginatedList<Credential>> GetAll(Guid folderId, int pageNumber, int pageSize,
        CancellationToken ct = default)
    {
        var query = _dbContext.Credentials.AsNoTracking().AsQueryable().Where(x => x.FolderId == folderId);
        var totalCount = await query.CountAsync(ct);

        if (totalCount == 0)
        {
            return new PaginatedList<Credential>(0, pageSize, 0, []);
        }

        var skip = (pageNumber - 1) * pageSize;
        var items = await query.OrderBy(x => x.Position).Skip(skip).Take(pageSize).ToListAsync(ct);

        return new PaginatedList<Credential>(pageNumber, pageSize, totalCount, items);
    }

    public Task<string?> GetPassword(long credentialId, CancellationToken ct = default)
    {
        return _dbContext.Credentials.Where(x => x.Id == credentialId).Select(x => x.Password)
            .SingleOrDefaultAsync(ct);
    }

    public Task<List<Credential>> GetUserCredentials(long userId, CancellationToken ct = default)
    {
        return _dbContext.Credentials.Where(x => x.Folder.UserId == userId).ToListAsync(ct);
    }

    public async Task<long> GetNextAvailablePosition(Guid folderId, CancellationToken ct = default)
    {
        var lastPosition = await _dbContext.Credentials.Where(x => x.FolderId == folderId)
            .MaxAsync(x => (long?)x.Position, ct);
        return lastPosition.HasValue ? lastPosition.Value + 1 : 1;
    }

    public Task<bool> IsCredentialOwnedByUser(long userId, long credentialId, CancellationToken ct = default)
    {
        return _dbContext.Credentials.AnyAsync(x => x.Folder.UserId == userId && x.Id == credentialId, ct);
    }

    public async Task ExecuteUpdateFolder(long credentialId, Guid newFolderId, CancellationToken ct = default)
    {
        var newPosition = await GetNextAvailablePosition(newFolderId, ct);

        await _dbContext.Credentials.Where(x => x.Id == credentialId).ExecuteUpdateAsync(x => x
            .SetProperty(c => c.FolderId, newFolderId)
            .SetProperty(c => c.Position, newPosition), ct);
    }

    public async Task ExecuteUpdatePosition(long credentialId, long newPosition, CancellationToken ct = default)
    {
        var (folderId, oldPosition) = await GetFolderIdAndPosition(credentialId, ct);
        if (oldPosition == newPosition)
        {
            return;
        }

        if (newPosition < oldPosition)
        {
            await _dbContext.Credentials.Where(x => x.FolderId == folderId && x.Position >= newPosition)
                .ExecuteUpdateAsync(x => x.SetProperty(c => c.Position, c => c.Position + 1), ct);
        }
        else
        {
            await _dbContext.Credentials.Where(x => x.FolderId == folderId && x.Position <= newPosition)
                .ExecuteUpdateAsync(x => x.SetProperty(c => c.Position, c => c.Position - 1), ct);
        }

        await _dbContext.Credentials.Where(x => x.Id == credentialId)
            .ExecuteUpdateAsync(x => x.SetProperty(c => c.Position, newPosition), ct);
    }

    private async Task<(Guid folderId, long position)> GetFolderIdAndPosition(long credentialId,
        CancellationToken ct = default)
    {
        var result = (await _dbContext.Credentials.Where(x => x.Id == credentialId)
            .Select(x => new { x.FolderId, x.Position }).SingleOrDefaultAsync(ct))!;
        return (result.FolderId, result.Position);
    }
}