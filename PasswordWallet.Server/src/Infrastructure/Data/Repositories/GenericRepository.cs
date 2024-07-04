using System.Linq.Expressions;
using Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class GenericRepository<TEntity, TKey>(PasswordWalletDbContext dbContext) : IGenericRepository<TEntity, TKey>
    where TEntity : class
{
    private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

    public virtual async Task<IReadOnlyCollection<TEntity>> GetAll(CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking().ToListAsync(ct);
    }

    public virtual ValueTask<TEntity?> Get(TKey id, CancellationToken ct = default)
    {
        return dbContext.FindAsync<TEntity>([id], cancellationToken: ct);
    }

    public virtual Task<TEntity?> GetSingleWithoutTracking(Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default)
    {
        return _dbSet.AsNoTracking().SingleOrDefaultAsync(predicate, ct);
    }

    public virtual Task Add(TEntity entity, CancellationToken ct = default)
    {
        _dbSet.Add(entity);
        return Task.CompletedTask;
    }

    public virtual Task Delete(TEntity entity, CancellationToken ct = default)
    {
        if (dbContext.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }

        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual Task ExecuteDelete(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
    {
        return _dbSet.Where(predicate).ExecuteDeleteAsync(ct);
    }

    public virtual Task<bool> Exists(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
    {
        return _dbSet.AnyAsync(predicate, ct);
    }

    public virtual Task<int> Count(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default)
    {
        return predicate is null ? _dbSet.CountAsync(ct) : _dbSet.CountAsync(predicate, ct);
    }
}