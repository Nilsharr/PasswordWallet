using FastEndpoints;

namespace Api.Endpoints.v1.Folders.AddFolder;

public record AddFolderRequest(string Name, [property: FromClaim] long UserId = 0);