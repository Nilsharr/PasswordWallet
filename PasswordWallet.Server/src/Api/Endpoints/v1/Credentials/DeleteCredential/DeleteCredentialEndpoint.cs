using Api.Config.Groups;
using Core.Constants;
using Core.Interfaces.Repositories;
using FastEndpoints;

namespace Api.Endpoints.v1.Credentials.DeleteCredential;

public class DeleteCredentialEndpoint(IUnitOfWork unitOfWork) : Endpoint<DeleteCredentialRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete($"/{{{RouteConstants.CredentialIdParam}:long}}");
        Policies(AuthenticationConstants.UserPolicy);
        PreProcessors(new VerifyCredentialOwnerPreProcessor<DeleteCredentialRequest>());
        Group<CredentialGroup>();
        Description(x => x.ClearDefaultProduces(200).Produces(204).Produces(404));
    }

    public override async Task HandleAsync(DeleteCredentialRequest req, CancellationToken ct)
    {
        await unitOfWork.CredentialRepository.ExecuteDelete(x => x.Id == req.CredentialId, ct);
        await SendNoContentAsync(ct);
    }
}