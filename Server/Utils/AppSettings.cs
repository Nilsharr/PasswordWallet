namespace PasswordWallet.Server.Utils;

public class AppSettings
{
    public string JwtSigningKey { get; set; } = default!;
    public string PasswordPepper { get; set; } = default!;
}