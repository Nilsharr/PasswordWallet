using Api.Config.Groups;
using Core.Constants;
using Core.Interfaces.Repositories;
using FastEndpoints;

namespace Api.Endpoints.v1.Folders.DeleteFolder;

public class DeleteFolderEndpoint(IUnitOfWork unitOfWork) : Endpoint<DeleteFolderRequest>
{
    public override void Configure()
    {
        Delete($"/{{{RouteConstants.FolderIdParam}:guid}}");
        Policies(AuthenticationConstants.UserPolicy);
        PreProcessors(new VerifyFolderOwnerPreProcessor<DeleteFolderRequest>());
        Group<FolderGroup>();
        Description(x => x.ClearDefaultProduces(200).Produces(204).Produces(404));
    }

    public override async Task HandleAsync(DeleteFolderRequest req, CancellationToken ct)
    {
        await unitOfWork.FolderRepository.ExecuteDelete(x => x.Id == req.FolderId, ct);
        await SendNoContentAsync(ct);
    }
}