using Core.Entities;
using Core.Enums;
using Core.Models;

namespace Core.Interfaces.Repositories;

public interface ILoginHistoryRepository : IGenericRepository<LoginHistory, long>
{
    Task<PaginatedList<LoginHistory>> GetAll(long userId, int pageNumber = 1, int pageSize = 20,
        SortDirection sortDirection = SortDirection.Desc, bool? correct = null, CancellationToken ct = default);

    Task<(DateTimeOffset? lastValid, DateTimeOffset? lastInvalid)> GetLastLoginDates(long userId,
        CancellationToken ct = default);

    Task<(int valid, int invalid)> GetAmountOfLogins(long userId, CancellationToken ct = default);
}