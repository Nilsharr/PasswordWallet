using Api.Config.Groups;
using Core.Constants;
using Core.Interfaces.Repositories;
using FastEndpoints;

namespace Api.Endpoints.v1.Credentials.GetCredential;

public class GetCredentialEndpoint(IUnitOfWork unitOfWork)
    : Endpoint<GetCredentialRequest, CredentialResponse, CredentialMapper>
{
    public override void Configure()
    {
        Get($"/{{{RouteConstants.CredentialIdParam}:long}}");
        Policies(AuthenticationConstants.UserPolicy);
        PreProcessors(new VerifyCredentialOwnerPreProcessor<GetCredentialRequest>());
        Group<CredentialGroup>();
        Description(x => x.Produces(404));
    }

    public override async Task HandleAsync(GetCredentialRequest req, CancellationToken ct)
    {
        var credential = (await unitOfWork.CredentialRepository.Get(req.CredentialId, ct))!;
        await SendOkAsync(Map.FromEntity(credential), cancellation: ct);
    }
}