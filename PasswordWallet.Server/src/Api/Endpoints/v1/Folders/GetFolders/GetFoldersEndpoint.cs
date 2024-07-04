using Api.Config.Groups;
using Core.Constants;
using Core.Interfaces.Repositories;
using FastEndpoints;

namespace Api.Endpoints.v1.Folders.GetFolders;

public class GetFoldersEndpoint(IUnitOfWork unitOfWork)
    : Endpoint<GetFoldersRequest, IEnumerable<FolderResponse>, FolderMapper>
{
    public override void Configure()
    {
        Get("/");
        Policies(AuthenticationConstants.UserPolicy);
        Group<FolderGroup>();
    }

    public override async Task HandleAsync(GetFoldersRequest req, CancellationToken ct)
    {
        var folders = await unitOfWork.FolderRepository.GetAll(req.UserId, ct);
        await SendOkAsync(Map.FromEntities(folders), ct);
    }
}