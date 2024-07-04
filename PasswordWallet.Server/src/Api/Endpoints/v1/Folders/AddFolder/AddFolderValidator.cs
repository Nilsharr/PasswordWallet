using FastEndpoints;
using FluentValidation;

namespace Api.Endpoints.v1.Folders.AddFolder;

public class AddFolderValidator : Validator<AddFolderRequest>
{
    public AddFolderValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(64)
            .WithMessage("Name cannot have more than 64 characters.");
    }
}