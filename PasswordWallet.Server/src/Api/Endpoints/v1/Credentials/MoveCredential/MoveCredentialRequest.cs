namespace Api.Endpoints.v1.Credentials.MoveCredential;

public record MoveCredentialRequest(long CredentialId, Guid FolderId);