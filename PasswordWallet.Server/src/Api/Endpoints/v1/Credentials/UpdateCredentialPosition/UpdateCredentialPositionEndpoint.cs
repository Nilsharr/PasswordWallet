using Api.Config.Groups;
using Core.Constants;
using Core.Interfaces.Repositories;
using FastEndpoints;

namespace Api.Endpoints.v1.Credentials.UpdateCredentialPosition;

public class UpdateCredentialPositionEndpoint(IUnitOfWork unitOfWork) : Endpoint<UpdateCredentialPositionRequest>
{
    public override void Configure()
    {
        Patch($"/{{{RouteConstants.CredentialIdParam}:long}}/position");
        Policies(AuthenticationConstants.UserPolicy);
        PreProcessors(new VerifyCredentialOwnerPreProcessor<UpdateCredentialPositionRequest>());
        Group<CredentialGroup>();
        Description(x => x.ClearDefaultProduces(200).Produces(204).Produces(404));
    }

    public override async Task HandleAsync(UpdateCredentialPositionRequest req, CancellationToken ct)
    {
        await unitOfWork.CredentialRepository.ExecuteUpdatePosition(req.CredentialId, req.Position, ct);
        await SendNoContentAsync(ct);
    }
}