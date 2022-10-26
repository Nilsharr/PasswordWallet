using FluentValidation;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.Server.Validators;

public class SignupValidator : Validator<SignupRequestDto>
{
    public SignupValidator()
    {
        // TODO add validation rule for login already in use
        RuleFor(x => x.Login).NotEmpty().WithMessage("Login is required!").MaximumLength(30)
            .WithMessage("Login cannot have more than 30 characters");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required!").Equal(x => x.ConfirmPassword)
            .WithMessage("Passwords do not match!");
        RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("You must repeat password");
    }
}