using FastEndpoints;

namespace Api.Endpoints.v1.Folders.GetFolders;

public record GetFoldersRequest([property: FromClaim] long UserId = 0);