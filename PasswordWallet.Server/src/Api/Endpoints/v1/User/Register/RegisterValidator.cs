using Core.Interfaces.Repositories;
using FastEndpoints;
using FluentValidation;

namespace Api.Endpoints.v1.User.Register;

public class RegisterValidator : Validator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.").MaximumLength(30)
            .WithMessage("Username cannot have more than 30 characters.").MustAsync(BeUniqueUsername)
            .WithMessage("Username is already taken.");

        Include(new PasswordConfirmationValidator());
    }

    private async Task<bool> BeUniqueUsername(string username, CancellationToken ct)
    {
        var unitOfWork = Resolve<IUnitOfWork>();
        return !await unitOfWork.UserRepository.ExistsWithUsername(username, ct);
    }
}