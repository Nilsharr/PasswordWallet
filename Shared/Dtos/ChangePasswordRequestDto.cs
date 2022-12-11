using FastEndpoints;

namespace PasswordWallet.Shared.Dtos;

public class ChangePasswordRequestDto
{
    [FromClaim] public long? AccountId { get; set; }
    public string Password { get; set; } = default!;
    public string ConfirmPassword { get; set; } = default!;
    public bool IsPasswordKeptAsHash { get; set; }
}