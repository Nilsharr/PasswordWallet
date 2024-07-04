using Api.Config.Groups;
using Api.Endpoints.v1.Credentials.GetCredential;
using Api.Endpoints.v1.Folders;
using Core.Constants;
using Core.Interfaces.Services;
using FastEndpoints;

namespace Api.Endpoints.v1.Credentials.AddCredential;

public class AddCredentialEndpoint(ICredentialService credentialService)
    : Endpoint<AddCredentialRequest, CredentialResponse, CredentialMapper>

{
    public override void Configure()
    {
        Post($"/folders/{{{RouteConstants.FolderIdParam}:guid}}");
        Policies(AuthenticationConstants.UserPolicy);
        PreProcessors(new VerifyFolderOwnerPreProcessor<AddCredentialRequest>());
        Group<CredentialGroup>();
        Description(x => x.ClearDefaultProduces(200).Produces<CredentialResponse>(201).Produces(404));
    }

    public override async Task HandleAsync(AddCredentialRequest req, CancellationToken ct)
    {
        var entity = Map.ToEntity(req);
        var credential = await credentialService.EncryptAndSaveCredential(req.UserId, entity, ct);
        await SendCreatedAtAsync<GetCredentialEndpoint>(credential.Id, Map.FromEntity(credential), cancellation: ct);
    }
}