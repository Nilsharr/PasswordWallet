using FastEndpoints;
using FluentValidation;

namespace Api.Endpoints.v1.User.ChangePassword;

public class ChangePasswordValidator : Validator<ChangePasswordRequest>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("Current password is required.");
        Include(new PasswordConfirmationValidator());
    }
}