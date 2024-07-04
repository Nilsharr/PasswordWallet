using Api.Extensions;
using Core.Constants;
using Core.Interfaces.Repositories;
using FastEndpoints;

namespace Api.Endpoints.v1.Credentials;

public class VerifyCredentialOwnerPreProcessor<TRequest> : IPreProcessor<TRequest>
{
    public async Task PreProcessAsync(IPreProcessorContext<TRequest> context, CancellationToken ct)
    {
        if (context.HttpContext.ResponseStarted())
        {
            return;
        }

        var credentialId = Convert.ToInt64(context.HttpContext.Request.RouteValues[RouteConstants.CredentialIdParam]);
        var userId = context.HttpContext.User.GetUserId()!.Value;
        var unitOfWork = context.HttpContext.Resolve<IUnitOfWork>();

        var exists = await unitOfWork.CredentialRepository.Exists(x => x.Id == credentialId, ct);
        if (!exists)
        {
            await context.HttpContext.Response.SendNotFoundAsync(ct);
            return;
        }

        var isOwned = await unitOfWork.CredentialRepository.IsCredentialOwnedByUser(userId, credentialId, ct);
        if (!isOwned)
        {
            await context.HttpContext.Response.SendForbiddenAsync(ct);
        }
    }
}