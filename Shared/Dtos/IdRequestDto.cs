using FastEndpoints;

namespace PasswordWallet.Shared.Dtos;

public class IdRequestDto
{
    [FromClaim] public long? AccountId { get; set; }
    public long Id { get; set; }
}