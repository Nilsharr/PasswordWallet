namespace PasswordWallet.Shared.Dtos;

public class ChangePasswordRequestDto
{
    public string Password { get; set; } = default!;
    public string ConfirmPassword { get; set; } = default!;
    public bool IsPasswordKeptAsHash { get; set; }
}