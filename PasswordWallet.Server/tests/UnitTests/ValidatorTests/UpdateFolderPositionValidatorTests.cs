using Api.Endpoints.v1.Folders.UpdateFolderPosition;
using FluentValidation.TestHelper;

namespace UnitTests.ValidatorTests;

public class UpdateFolderPositionValidatorTests
{
    private readonly UpdateFolderPositionValidator _updateFolderPositionValidator = new();

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    public void PositionIsPositiveNumber_ShouldValidate(int position)
    {
        var positionRequest = new UpdateFolderPositionRequest(Guid.NewGuid(), position, 1);

        var result = _updateFolderPositionValidator.TestValidate(positionRequest);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-3)]
    public void PositionIsNotPositiveNumber_ShouldNotValidate(int position)
    {
        var positionRequest = new UpdateFolderPositionRequest(Guid.NewGuid(), position, 1);

        var result = _updateFolderPositionValidator.TestValidate(positionRequest);

        result.ShouldHaveValidationErrorFor(x => x.Position);
    }
}