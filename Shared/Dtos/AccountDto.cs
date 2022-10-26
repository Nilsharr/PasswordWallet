namespace PasswordWallet.Shared.Dtos;

public class AccountDto
{
    public int Id { get; set; }
    public string Login { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string? Salt { get; set; }
    public bool IsPasswordKeptAsHash { get; set; }

    //public List<CredentialsDto> Credentials { get; set; } = default!;
}