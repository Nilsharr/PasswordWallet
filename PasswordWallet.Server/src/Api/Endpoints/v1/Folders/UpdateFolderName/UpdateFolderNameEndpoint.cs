using Api.Config.Groups;
using Core.Constants;
using Core.Interfaces.Repositories;
using FastEndpoints;

namespace Api.Endpoints.v1.Folders.UpdateFolderName;

public class UpdateFolderNameEndpoint(IUnitOfWork unitOfWork)
    : Endpoint<UpdateFolderNameRequest, FolderMapper>
{
    public override void Configure()
    {
        Patch($"/{{{RouteConstants.FolderIdParam}:guid}}/name");
        Policies(AuthenticationConstants.UserPolicy);
        PreProcessors(new VerifyFolderOwnerPreProcessor<UpdateFolderNameRequest>());
        Group<FolderGroup>();
        Description(x => x.ClearDefaultProduces(200).Produces(204).Produces(404));
    }

    public override async Task HandleAsync(UpdateFolderNameRequest req, CancellationToken ct)
    {
        await unitOfWork.FolderRepository.ExecuteUpdateName(req.FolderId, req.Name, ct);
        await SendNoContentAsync(ct);
    }
}