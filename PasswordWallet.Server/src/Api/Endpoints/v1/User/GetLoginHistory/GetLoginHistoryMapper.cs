using Core.Entities;
using Core.Models;
using FastEndpoints;

namespace Api.Endpoints.v1.User.GetLoginHistory;

public class GetLoginHistoryMapper : ResponseMapper<GetLoginHistoryResponse, LoginHistory>
{
    public PaginatedList<GetLoginHistoryResponse> FromEntities(PaginatedList<LoginHistory> paginatedList) => new(
        paginatedList.PageNumber, paginatedList.PageSize, paginatedList.TotalCount,
        FromEntities(paginatedList.Items));

    private static List<GetLoginHistoryResponse> FromEntities(IEnumerable<LoginHistory> loginHistories) =>
        loginHistories.Select(h => new GetLoginHistoryResponse(h.Id, h.Date, h.Correct, h.IpAddress)).ToList();
}