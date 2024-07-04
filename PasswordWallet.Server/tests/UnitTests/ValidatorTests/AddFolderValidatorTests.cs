using Api.Endpoints.v1.Folders.AddFolder;
using FluentValidation.TestHelper;

namespace UnitTests.ValidatorTests;

public class AddFolderValidatorTests
{
    private readonly AddFolderValidator _addFolderValidator = new();

    [Theory]
    [InlineData("medklun")]
    [InlineData("test234")]
    [InlineData("plktgy")]
    public void ValidFolder_ShouldValidate(string name)
    {
        var folderRequest = new AddFolderRequest(name);

        var result = _addFolderValidator.TestValidate(folderRequest);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    [InlineData("           ")]
    public void EmptyName_ShouldNotValidate(string? name)
    {
        var folderRequest = new AddFolderRequest(name!);

        var result = _addFolderValidator.TestValidate(folderRequest);

        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Theory]
    [InlineData(65)]
    [InlineData(90)]
    [InlineData(114)]
    public void TooLongName_ShouldNotValidate(int stringLength)
    {
        var folderRequest = new AddFolderRequest(new string('n', stringLength));

        var result = _addFolderValidator.TestValidate(folderRequest);

        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }
}