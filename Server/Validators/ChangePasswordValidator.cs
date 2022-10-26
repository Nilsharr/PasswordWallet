using FluentValidation;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.Server.Validators;

public class ChangePasswordValidator : Validator<ChangePasswordRequestDto>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required!").Equal(x => x.ConfirmPassword)
            .WithMessage("Passwords do not match!");
        RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("You must repeat password");
    }
}