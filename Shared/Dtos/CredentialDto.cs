using FastEndpoints;

namespace PasswordWallet.Shared.Dtos;

public class CredentialDto
{
    public long Id { get; set; }
    public string Password { get; set; } = default!;
    public string? Login { get; set; }
    public string? WebAddress { get; set; }
    public string? Description { get; set; }
    [FromClaim] public long? AccountId { get; set; }
}