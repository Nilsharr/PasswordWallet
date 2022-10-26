using FluentValidation;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.Server.Validators;

public class LoginValidator : Validator<LoginRequestDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Login).NotEmpty().WithMessage("Login is required!").MaximumLength(30)
            .WithMessage("Login cannot have more than 30 characters");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required!");
    }
}