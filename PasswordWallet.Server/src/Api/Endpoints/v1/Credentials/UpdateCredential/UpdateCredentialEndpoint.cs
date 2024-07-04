using Api.Config.Groups;
using Core.Constants;
using Core.Interfaces.Services;
using FastEndpoints;

namespace Api.Endpoints.v1.Credentials.UpdateCredential;

public class UpdateCredentialEndpoint(ICredentialService credentialService)
    : Endpoint<UpdateCredentialRequest, CredentialResponse, CredentialMapper>
{
    public override void Configure()
    {
        Put($"/{{{RouteConstants.CredentialIdParam}:long}}");
        Policies(AuthenticationConstants.UserPolicy);
        PreProcessors(new VerifyCredentialOwnerPreProcessor<UpdateCredentialRequest>());
        Group<CredentialGroup>();
        Description(x => x.Produces(404));
    }

    public override async Task HandleAsync(UpdateCredentialRequest req, CancellationToken ct)
    {
        var entity = Map.ToEntity(req);
        var updated = await credentialService.EncryptAndUpdateCredential(req.UserId, entity, ct);
        await SendOkAsync(Map.FromEntity(updated), ct);
    }
}