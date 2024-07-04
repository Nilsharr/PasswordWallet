using FastEndpoints;
using FluentValidation;

namespace Api.Endpoints.v1.Folders.UpdateFolderName;

public class UpdateFolderNameValidator : Validator<UpdateFolderNameRequest>
{
    public UpdateFolderNameValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(64)
            .WithMessage("Name cannot have more than 64 characters.");
    }
}