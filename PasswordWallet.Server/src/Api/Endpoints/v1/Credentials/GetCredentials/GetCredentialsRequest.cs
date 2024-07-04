using FastEndpoints;

namespace Api.Endpoints.v1.Credentials.GetCredentials;

public record GetCredentialsRequest(
    Guid FolderId,
    [property: QueryParam] int PageNumber = 1,
    [property: QueryParam] int PageSize = 20);