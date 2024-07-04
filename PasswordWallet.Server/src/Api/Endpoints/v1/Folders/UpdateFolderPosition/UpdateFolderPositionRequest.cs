using FastEndpoints;

namespace Api.Endpoints.v1.Folders.UpdateFolderPosition;

public record UpdateFolderPositionRequest(Guid FolderId, long Position, [property: FromClaim] long UserId = 0);