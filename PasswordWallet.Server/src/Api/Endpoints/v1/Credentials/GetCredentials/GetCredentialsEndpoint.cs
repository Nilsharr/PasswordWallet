using Api.Config.Groups;
using Api.Endpoints.v1.Folders;
using Core.Constants;
using Core.Interfaces.Repositories;
using Core.Models;
using FastEndpoints;

namespace Api.Endpoints.v1.Credentials.GetCredentials;

public class GetCredentialsEndpoint(IUnitOfWork unitOfWork)
    : Endpoint<GetCredentialsRequest, PaginatedList<CredentialResponse>, CredentialMapper>
{
    public override void Configure()
    {
        Get($"/folders/{{{RouteConstants.FolderIdParam}:guid}}");
        Policies(AuthenticationConstants.UserPolicy);
        PreProcessors(new VerifyFolderOwnerPreProcessor<GetCredentialsRequest>());
        Group<CredentialGroup>();
        Description(x => x.Produces(404));
    }

    public override async Task HandleAsync(GetCredentialsRequest req, CancellationToken ct)
    {
        var result = await unitOfWork.CredentialRepository.GetAll(req.FolderId, req.PageNumber, req.PageSize, ct);
        await SendOkAsync(Map.FromEntities(result), cancellation: ct);
    }
}