using Core.Interfaces.Repositories;

namespace Infrastructure.Data.Repositories;

public sealed class UnitOfWork(PasswordWalletDbContext dbContext) : IUnitOfWork
{
    private bool _disposed;
    private IUserRepository? _userRepository;

    public IUserRepository UserRepository => _userRepository ??= new UserRepository(dbContext);

    private ICredentialRepository? _credentialRepository;

    public ICredentialRepository CredentialRepository =>
        _credentialRepository ??= new CredentialRepository(dbContext);

    private IFolderRepository? _folderRepository;

    public IFolderRepository FolderRepository => _folderRepository ??= new FolderRepository(dbContext);

    private ILoginHistoryRepository? _loginHistoryRepository;

    public ILoginHistoryRepository LoginHistoryRepository =>
        _loginHistoryRepository ??= new LoginHistoryRepository(dbContext);

    public Task SaveChangesAsync(CancellationToken ct = default)
    {
        return dbContext.SaveChangesAsync(ct);
    }

    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                dbContext.Dispose();
            }
        }

        _disposed = true;
    }

    public ValueTask DisposeAsync()
    {
        return DisposeAsync(true);
    }

    private async ValueTask DisposeAsync(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                await dbContext.DisposeAsync();
            }
        }

        _disposed = true;
    }
}