using FastEndpoints;
using FluentValidation;

namespace Api.Endpoints.v1.Credentials.UpdateCredentialPosition;

public class UpdateCredentialPositionValidator : Validator<UpdateCredentialPositionRequest>
{
    public UpdateCredentialPositionValidator()
    {
        RuleFor(x => x.Position).GreaterThan(0).WithMessage("Position cannot be a negative number.");
    }
}