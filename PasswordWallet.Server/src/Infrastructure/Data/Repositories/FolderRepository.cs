using Core.Entities;
using Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class FolderRepository(PasswordWalletDbContext dbContext)
    : GenericRepository<Folder, Guid>(dbContext), IFolderRepository
{
    private readonly PasswordWalletDbContext _dbContext = dbContext;

    public async Task<IReadOnlyCollection<Folder>> GetAll(long userId, CancellationToken ct = default)
    {
        return await _dbContext.Folders.AsNoTracking().Where(x => x.UserId == userId).OrderBy(x => x.Position)
            .ToListAsync(ct);
    }

    public async Task<long> GetNextAvailablePosition(long userId, CancellationToken ct = default)
    {
        var lastPosition = await _dbContext.Folders.Where(x => x.UserId == userId).MaxAsync(x => (long?)x.Position, ct);
        return lastPosition.HasValue ? lastPosition.Value + 1 : 1;
    }

    public Task<bool> IsFolderOwnedByUser(long userId, Guid folderId, CancellationToken ct = default)
    {
        return _dbContext.Folders.AnyAsync(x => x.UserId == userId && x.Id == folderId, ct);
    }

    public Task ExecuteUpdateName(Guid folderId, string name, CancellationToken ct = default)
    {
        return _dbContext.Folders.Where(x => x.Id == folderId)
            .ExecuteUpdateAsync(x => x.SetProperty(f => f.Name, name), ct);
    }

    public async Task ExecuteUpdatePosition(long userId, Guid folderId, long newPosition,
        CancellationToken ct = default)
    {
        var oldPosition = await _dbContext.Folders.Where(x => x.Id == folderId).Select(x => x.Position)
            .SingleOrDefaultAsync(ct);
        if (oldPosition == newPosition)
        {
            return;
        }

        if (newPosition < oldPosition)
        {
            await _dbContext.Folders.Where(x => x.UserId == userId && x.Position >= newPosition)
                .ExecuteUpdateAsync(x => x.SetProperty(f => f.Position, f => f.Position + 1), ct);
        }
        else
        {
            await _dbContext.Folders.Where(x => x.UserId == userId && x.Position <= newPosition)
                .ExecuteUpdateAsync(x => x.SetProperty(f => f.Position, f => f.Position - 1), ct);
        }

        await _dbContext.Folders.Where(x => x.Id == folderId)
            .ExecuteUpdateAsync(x => x.SetProperty(f => f.Position, newPosition), ct);
    }
}