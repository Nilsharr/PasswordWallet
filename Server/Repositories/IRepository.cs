namespace PasswordWallet.Server.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> Get(long id, CancellationToken ct = default);
    T Add(T entity);
    T Update(T entity);
    Task Delete(long id, CancellationToken ct = default);
}