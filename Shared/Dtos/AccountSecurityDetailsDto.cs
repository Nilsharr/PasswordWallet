namespace PasswordWallet.Shared.Dtos;

public class AccountSecurityDetailsDto
{
    public IList<LoginIpAddressDto> LoginRequest { get; set; } = new List<LoginIpAddressDto>();
    public DateTime? LastSuccessfulLogin { get; set; }
    public DateTime? LastUnsuccessfulLogin { get; set; }
}