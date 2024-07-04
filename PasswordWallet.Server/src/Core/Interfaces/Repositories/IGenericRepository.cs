using System.Linq.Expressions;

namespace Core.Interfaces.Repositories;

public interface IGenericRepository<TEntity, in TKey> where TEntity : class
{
    Task<IReadOnlyCollection<TEntity>> GetAll(CancellationToken ct = default);
    ValueTask<TEntity?> Get(TKey id, CancellationToken ct = default);
    Task<TEntity?> GetSingleWithoutTracking(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
    Task Add(TEntity entity, CancellationToken ct = default);
    Task Delete(TEntity entity, CancellationToken ct = default);
    Task ExecuteDelete(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
    Task<bool> Exists(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
    Task<int> Count(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);
}