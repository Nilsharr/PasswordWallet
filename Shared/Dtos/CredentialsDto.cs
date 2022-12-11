using FastEndpoints;

namespace PasswordWallet.Shared.Dtos;

public class CredentialsDto
{
    public long Id { get; set; }
    public string Password { get; set; } = default!;
    public string? DecryptedPassword { get; set; }
    public string? Login { get; set; }
    public string? WebAddress { get; set; }
    public string? Description { get; set; }
    [FromClaim] public long? AccountId { get; set; }

    public CredentialsDto(CredentialsDto credentialsDto)
    {
        Id = credentialsDto.Id;
        Password = credentialsDto.Password;
        Login = credentialsDto.Login;
        WebAddress = credentialsDto.WebAddress;
        Description = credentialsDto.Description;
    }

    public CredentialsDto()
    {
    }
}