using FastEndpoints;
using FluentValidation;

namespace Api.Endpoints.v1.Credentials.AddCredential;

public class AddCredentialValidator : Validator<AddCredentialRequest>
{
    public AddCredentialValidator()
    {
        RuleFor(x => x.Username).MaximumLength(60).WithMessage("Username cannot have more than 60 characters.");
        RuleFor(x => x.WebAddress).MaximumLength(256).WithMessage("Web Address cannot have more than 256 characters.");
        RuleFor(x => x.Description).MaximumLength(512).WithMessage("Description cannot have more than 512 characters.");
    }
}