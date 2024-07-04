using Api.Config.Groups;
using Api.Endpoints.v1.Folders.GetFolder;
using Core.Constants;
using Core.Interfaces.Repositories;
using FastEndpoints;

namespace Api.Endpoints.v1.Folders.AddFolder;

public class AddFolderEndpoint(IUnitOfWork unitOfWork) : Endpoint<AddFolderRequest, FolderResponse, FolderMapper>
{
    public override void Configure()
    {
        Post("/");
        Policies(AuthenticationConstants.UserPolicy);
        Group<FolderGroup>();
        Description(x => x.ClearDefaultProduces(200).Produces<AddFolderRequest>(201));
    }

    public override async Task HandleAsync(AddFolderRequest req, CancellationToken ct)
    {
        var folder = Map.ToEntity(req);
        folder.Position = await unitOfWork.FolderRepository.GetNextAvailablePosition(req.UserId, ct);
        await unitOfWork.FolderRepository.Add(folder, ct);
        await unitOfWork.SaveChangesAsync(ct);
        await SendCreatedAtAsync<GetFolderEndpoint>(folder.Id, Map.FromEntity(folder), cancellation: ct);
    }
}