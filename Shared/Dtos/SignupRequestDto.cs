namespace PasswordWallet.Shared.Dtos;

public class SignupRequestDto : LoginRequestDto
{
    public string ConfirmPassword { get; set; } = default!;
    public bool IsPasswordKeptAsHash { get; set; }
}