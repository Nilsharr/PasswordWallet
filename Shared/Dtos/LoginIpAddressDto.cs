using System.Net;

namespace PasswordWallet.Shared.Dtos;

public class LoginIpAddressDto
{
    public long Id { get; set; }
    public IPAddress IpAddress { get; set; } = default!;
    public long AmountOfGoodLogins { get; set; }
    public long AmountOfBadLogins { get; set; }
    public bool PermanentLock { get; set; }
}