namespace Core.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    public IUserRepository UserRepository { get; }
    public ICredentialRepository CredentialRepository { get; }
    public IFolderRepository FolderRepository { get; }
    public ILoginHistoryRepository LoginHistoryRepository { get; }
    Task SaveChangesAsync(CancellationToken ct = default);
}