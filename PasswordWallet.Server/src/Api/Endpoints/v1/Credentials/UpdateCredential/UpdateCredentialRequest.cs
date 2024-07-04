using FastEndpoints;

namespace Api.Endpoints.v1.Credentials.UpdateCredential;

public record UpdateCredentialRequest(
    long CredentialId,
    string? Username = null,
    string? Password = null,
    string? WebAddress = null,
    string? Description = null,
    [property: FromClaim] long UserId = 0);