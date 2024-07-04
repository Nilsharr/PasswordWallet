using Core.Entities;

namespace Core.Interfaces.Repositories;

public interface IFolderRepository : IGenericRepository<Folder, Guid>
{
    Task<IReadOnlyCollection<Folder>> GetAll(long userId, CancellationToken ct = default);
    Task<long> GetNextAvailablePosition(long userId, CancellationToken ct = default);
    Task<bool> IsFolderOwnedByUser(long userId, Guid folderId, CancellationToken ct = default);
    Task ExecuteUpdateName(Guid folderId, string name, CancellationToken ct = default);
    Task ExecuteUpdatePosition(long userId, Guid folderId, long newPosition, CancellationToken ct = default);
}