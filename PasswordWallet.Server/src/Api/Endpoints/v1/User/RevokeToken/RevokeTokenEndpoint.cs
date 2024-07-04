using Api.Config.Groups;
using Core.Constants;
using Core.Interfaces.Repositories;
using FastEndpoints;

namespace Api.Endpoints.v1.User.RevokeToken;

public class RevokeTokenEndpoint(IUnitOfWork unitOfWork) : Endpoint<RevokeTokenRequest>
{
    public override void Configure()
    {
        Delete("/revoke");
        Policies(AuthenticationConstants.UserPolicy);
        Group<UserGroup>();
        Description(x => x.ClearDefaultProduces(200).Produces(204));
    }

    public override async Task HandleAsync(RevokeTokenRequest req, CancellationToken ct)
    {
        await unitOfWork.UserRepository.ExecuteRevokeRefreshToken(req.UserId, ct);
        await SendNoContentAsync(ct);
    }
}