using FastEndpoints;
using FluentValidation;

namespace Api.Endpoints.v1.User;

public class PasswordConfirmationValidator : Validator<PasswordConfirmationRequest>
{
    public PasswordConfirmationValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(10).WithMessage("Password must be at least 10 characters long.")
            .Matches("[a-z]+").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[A-Z]+").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]+").WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]+").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Password confirmation is required.")
            .Equal(x => x.Password).WithMessage("Passwords do not match.");
    }
}