namespace Api.Endpoints.v1.Credentials;

public record CredentialResponse(long Id, string? Username, string? WebAddress, string? Description, long Position);