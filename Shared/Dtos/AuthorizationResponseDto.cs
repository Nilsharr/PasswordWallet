namespace PasswordWallet.Shared.Dtos;

public class AuthorizationResponseDto
{
    public string Token { get; init; } = default!;
    public DateTime? LastSuccessfulLogin { get; set; }
    public DateTime? LastUnsuccessfulLogin { get; set; }
}