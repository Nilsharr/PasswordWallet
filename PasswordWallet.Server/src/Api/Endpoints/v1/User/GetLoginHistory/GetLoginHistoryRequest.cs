using Core.Enums;
using FastEndpoints;

namespace Api.Endpoints.v1.User.GetLoginHistory;

public record GetLoginHistoryRequest(
    [property: QueryParam] int PageNumber = 1,
    [property: QueryParam] int PageSize = 20,
    [property: QueryParam] SortDirection SortDir = SortDirection.Desc,
    [property: QueryParam] bool? Correct = null,
    [property: FromClaim] long UserId = 0);