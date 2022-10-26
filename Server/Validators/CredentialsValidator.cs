using FluentValidation;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.Server.Validators;

public class CredentialsValidator : Validator<CredentialsDto>
{
    public CredentialsValidator()
    {
        RuleFor(x => x.Login).MaximumLength(30).WithMessage("Login cannot have more than 30 characters");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required!");
        RuleFor(x => x.WebAddress).MaximumLength(256).WithMessage("Web Address cannot have more than 256 characters");
        RuleFor(x => x.Description).MaximumLength(256).WithMessage("Description cannot have more than 256 characters");
    }
}