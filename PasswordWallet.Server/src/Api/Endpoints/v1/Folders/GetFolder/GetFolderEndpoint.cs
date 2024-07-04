using Api.Config.Groups;
using Core.Constants;
using Core.Interfaces.Repositories;
using FastEndpoints;

namespace Api.Endpoints.v1.Folders.GetFolder;

public class GetFolderEndpoint(IUnitOfWork unitOfWork) : Endpoint<GetFolderRequest, FolderResponse, FolderMapper>
{
    public override void Configure()
    {
        Get($"/{{{RouteConstants.FolderIdParam}:guid}}");
        Policies(AuthenticationConstants.UserPolicy);
        PreProcessors(new VerifyFolderOwnerPreProcessor<GetFolderRequest>());
        Group<FolderGroup>();
        Description(x => x.Produces(404));
    }

    public override async Task HandleAsync(GetFolderRequest req, CancellationToken ct)
    {
        var folder = (await unitOfWork.FolderRepository.Get(req.FolderId, ct))!;
        await SendOkAsync(Map.FromEntity(folder), ct);
    }
}