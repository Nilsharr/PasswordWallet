using Api.Config.Groups;
using Core.Constants;
using Core.Interfaces.Repositories;
using FastEndpoints;

namespace Api.Endpoints.v1.Folders.UpdateFolderPosition;

public class UpdateFolderPositionEndpoint(IUnitOfWork unitOfWork) : Endpoint<UpdateFolderPositionRequest>
{
    public override void Configure()
    {
        Patch($"/{{{RouteConstants.FolderIdParam}:guid}}/position");
        Policies(AuthenticationConstants.UserPolicy);
        PreProcessors(new VerifyFolderOwnerPreProcessor<UpdateFolderPositionRequest>());
        Group<FolderGroup>();
        Description(x => x.ClearDefaultProduces(200).Produces(204).Produces(404));
    }

    public override async Task HandleAsync(UpdateFolderPositionRequest req, CancellationToken ct)
    {
        await unitOfWork.FolderRepository.ExecuteUpdatePosition(req.UserId, req.FolderId, req.Position, ct);
        await SendNoContentAsync(ct);
    }
}