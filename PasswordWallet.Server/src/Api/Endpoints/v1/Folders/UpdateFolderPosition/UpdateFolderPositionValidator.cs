using FastEndpoints;
using FluentValidation;

namespace Api.Endpoints.v1.Folders.UpdateFolderPosition;

public class UpdateFolderPositionValidator : Validator<UpdateFolderPositionRequest>
{
    public UpdateFolderPositionValidator()
    {
        RuleFor(x => x.Position).GreaterThan(0).WithMessage("Position cannot be a negative number.");
    }
}