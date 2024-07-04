using FastEndpoints;

namespace Api.Endpoints.v1.Credentials.GetDecryptedCredential;

public record GetDecryptedCredentialRequest(long CredentialId, [property: FromClaim] long UserId = 0);