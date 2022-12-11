namespace PasswordWallet.Shared.Dtos;

public class AccountDto
{
    public long Id { get; set; }
    public string Login { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string? Salt { get; set; }
    public bool IsPasswordKeptAsHash { get; set; }
}