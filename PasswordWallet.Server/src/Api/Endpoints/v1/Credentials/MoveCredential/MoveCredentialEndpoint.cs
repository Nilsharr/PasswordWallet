using Api.Config.Groups;
using Api.Endpoints.v1.Folders;
using Core.Constants;
using Core.Interfaces.Repositories;
using FastEndpoints;

namespace Api.Endpoints.v1.Credentials.MoveCredential;

public class MoveCredentialEndpoint(IUnitOfWork unitOfWork) : Endpoint<MoveCredentialRequest>
{
    public override void Configure()
    {
        Patch($"/{{{RouteConstants.CredentialIdParam}:long}}/folders/{{{RouteConstants.FolderIdParam}:guid}}");
        Policies(AuthenticationConstants.UserPolicy);
        PreProcessors(new VerifyCredentialOwnerPreProcessor<MoveCredentialRequest>(),
            new VerifyFolderOwnerPreProcessor<MoveCredentialRequest>());
        Group<CredentialGroup>();
        Description(x => x.ClearDefaultProduces(200).Produces(204).Produces(404));
    }

    public override async Task HandleAsync(MoveCredentialRequest req, CancellationToken ct)
    {
        await unitOfWork.CredentialRepository.ExecuteUpdateFolder(req.CredentialId, req.FolderId, ct);
        await SendNoContentAsync(ct);
    }
}