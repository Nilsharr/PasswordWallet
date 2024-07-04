using Api.Config.Groups;
using Core.Constants;
using Core.Interfaces.Repositories;
using Core.Models;
using FastEndpoints;

namespace Api.Endpoints.v1.User.GetLoginHistory;

public class GetLoginHistoryEndpoint(IUnitOfWork unitOfWork)
    : Endpoint<GetLoginHistoryRequest, PaginatedList<GetLoginHistoryResponse>, GetLoginHistoryMapper>
{
    public override void Configure()
    {
        Get("/login-history");
        Policies(AuthenticationConstants.UserPolicy);
        Group<UserGroup>();
        Summary(s =>
        {
            s.RequestParam(r => r.SortDir, "Specify direction of the sort. By default sorted descending.");
            s.RequestParam(r => r.Correct, "Specify whether to show only correct or incorrect login attempts");
        });
    }

    public override async Task HandleAsync(GetLoginHistoryRequest req, CancellationToken ct)
    {
        var result =
            await unitOfWork.LoginHistoryRepository.GetAll(req.UserId, req.PageNumber, req.PageSize, req.SortDir,
                req.Correct, ct);

        await SendOkAsync(Map.FromEntities(result), ct);
    }
}