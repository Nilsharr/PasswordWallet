using Api.Endpoints.v1.Folders.UpdateFolderName;
using FluentValidation.TestHelper;

namespace UnitTests.ValidatorTests;

public class UpdateFolderNameValidatorTests
{
    private readonly UpdateFolderNameValidator _updateFolderNameValidator = new();

    [Theory]
    [InlineData("medklun")]
    [InlineData("test234")]
    [InlineData("plktgy")]
    public void ValidFolder_ShouldValidate(string name)
    {
        var folderRequest = new UpdateFolderNameRequest(Guid.NewGuid(), name);

        var result = _updateFolderNameValidator.TestValidate(folderRequest);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    [InlineData("           ")]
    public void EmptyName_ShouldNotValidate(string? name)
    {
        var folderRequest = new UpdateFolderNameRequest(Guid.NewGuid(), name!);

        var result = _updateFolderNameValidator.TestValidate(folderRequest);

        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Theory]
    [InlineData(65)]
    [InlineData(90)]
    [InlineData(114)]
    public void TooLongName_ShouldNotValidate(int stringLength)
    {
        var folderRequest = new UpdateFolderNameRequest(Guid.NewGuid(), new string('n', stringLength));

        var result = _updateFolderNameValidator.TestValidate(folderRequest);

        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }
}