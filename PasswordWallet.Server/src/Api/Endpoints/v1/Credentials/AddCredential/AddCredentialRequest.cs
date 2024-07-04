using FastEndpoints;

namespace Api.Endpoints.v1.Credentials.AddCredential;

public record AddCredentialRequest(
    Guid FolderId,
    string? Username = null,
    string? Password = null,
    string? WebAddress = null,
    string? Description = null,
    [property: FromClaim] long UserId = 0);