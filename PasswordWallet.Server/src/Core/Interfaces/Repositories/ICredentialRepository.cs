using Core.Entities;
using Core.Models;

namespace Core.Interfaces.Repositories;

public interface ICredentialRepository : IGenericRepository<Credential, long>
{
    Task<PaginatedList<Credential>> GetAll(Guid folderId, int pageNumber, int pageSize,
        CancellationToken ct = default);

    Task<string?> GetPassword(long credentialId, CancellationToken ct = default);
    Task<List<Credential>> GetUserCredentials(long userId, CancellationToken ct = default);
    Task<long> GetNextAvailablePosition(Guid folderId, CancellationToken ct = default);
    Task<bool> IsCredentialOwnedByUser(long userId, long credentialId, CancellationToken ct = default);
    Task ExecuteUpdateFolder(long credentialId, Guid newFolderId, CancellationToken ct = default);
    Task ExecuteUpdatePosition(long credentialId, long newPosition, CancellationToken ct = default);
}