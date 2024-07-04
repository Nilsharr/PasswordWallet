using Core.Entities;
using Core.Enums;
using Core.Interfaces.Repositories;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class LoginHistoryRepository(PasswordWalletDbContext dbContext)
    : GenericRepository<LoginHistory, long>(dbContext), ILoginHistoryRepository
{
    private readonly PasswordWalletDbContext _dbContext = dbContext;

    public async Task<PaginatedList<LoginHistory>> GetAll(long userId, int pageNumber = 1, int pageSize = 20,
        SortDirection sortDirection = SortDirection.Desc, bool? correct = null, CancellationToken ct = default)
    {
        var query = _dbContext.LoginHistories.AsNoTracking().AsQueryable().Where(x => x.UserId == userId);
        if (correct is not null)
        {
            query = query.Where(x => x.Correct == correct);
        }

        var totalCount = await query.CountAsync(ct);
        if (totalCount == 0)
        {
            return new PaginatedList<LoginHistory>(0, pageSize, 0, []);
        }

        query = sortDirection == SortDirection.Asc ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);

        var skip = (pageNumber - 1) * pageSize;
        var items = await query.Skip(skip).Take(pageSize).ToListAsync(ct);
        return new PaginatedList<LoginHistory>(pageNumber, pageSize, totalCount, items);
    }

    public async Task<(DateTimeOffset? lastValid, DateTimeOffset? lastInvalid)> GetLastLoginDates(long userId,
        CancellationToken ct = default)
    {
        var dates = await _dbContext.LoginHistories
            .Where(x => x.UserId == userId)
            .GroupBy(x => x.Correct)
            .Select(g => new
            {
                Correct = g.Key,
                Date = (DateTimeOffset?)g.Max(x => x.Date)
            })
            .ToDictionaryAsync(x => x.Correct, x => x.Date, ct);

        dates.TryGetValue(true, out var valid);
        dates.TryGetValue(false, out var invalid);
        return (valid, invalid);
    }

    public async Task<(int valid, int invalid)> GetAmountOfLogins(long userId, CancellationToken ct = default)
    {
        var logins = await _dbContext.LoginHistories
            .Where(x => x.UserId == userId)
            .GroupBy(x => x.Correct)
            .Select(g => new
            {
                Correct = g.Key,
                Count = g.Count()
            })
            .ToDictionaryAsync(x => x.Correct, x => x.Count, ct);

        logins.TryGetValue(true, out var valid);
        logins.TryGetValue(false, out var invalid);
        return (valid, invalid);
    }
}