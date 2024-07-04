using Api.Config.Groups;
using Core.Constants;
using Core.Interfaces.Services;
using FastEndpoints;

namespace Api.Endpoints.v1.Credentials.GetDecryptedCredential;

public class GetDecryptedCredentialEndpoint(ICredentialService credentialService)
    : Endpoint<GetDecryptedCredentialRequest, GetDecryptedCredentialResponse>
{
    public override void Configure()
    {
        Get($"/{{{RouteConstants.CredentialIdParam}:long}}/decrypt");
        Policies(AuthenticationConstants.UserPolicy);
        PreProcessors(new VerifyCredentialOwnerPreProcessor<GetDecryptedCredentialRequest>());
        Group<CredentialGroup>();
        Description(x => x.Produces(404));
    }

    public override async Task HandleAsync(GetDecryptedCredentialRequest req, CancellationToken ct)
    {
        var decrypted = await credentialService.DecryptCredentialPassword(req.UserId, req.CredentialId, ct);
        var response = new GetDecryptedCredentialResponse(decrypted);
        await SendOkAsync(response, cancellation: ct);
    }
}