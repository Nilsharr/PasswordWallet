using Api.Extensions;
using Core.Constants;
using Core.Interfaces.Repositories;
using FastEndpoints;

namespace Api.Endpoints.v1.Folders;

public class VerifyFolderOwnerPreProcessor<TRequest> : IPreProcessor<TRequest>
{
    public async Task PreProcessAsync(IPreProcessorContext<TRequest> context, CancellationToken ct)
    {
        if (context.HttpContext.ResponseStarted())
        {
            return;
        }

        var folderId = Guid.Parse((string)context.HttpContext.Request.RouteValues[RouteConstants.FolderIdParam]!);
        var userId = context.HttpContext.User.GetUserId()!.Value;
        var unitOfWork = context.HttpContext.Resolve<IUnitOfWork>();

        var exists = await unitOfWork.FolderRepository.Exists(x => x.Id == folderId, ct);
        if (!exists)
        {
            await context.HttpContext.Response.SendNotFoundAsync(ct);
            return;
        }

        var isOwned = await unitOfWork.FolderRepository.IsFolderOwnedByUser(userId, folderId, ct);
        if (!isOwned)
        {
            await context.HttpContext.Response.SendForbiddenAsync(ct);
        }
    }
}