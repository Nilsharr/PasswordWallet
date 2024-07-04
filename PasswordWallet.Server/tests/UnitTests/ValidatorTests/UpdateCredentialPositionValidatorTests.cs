using Api.Endpoints.v1.Credentials.UpdateCredentialPosition;
using FluentValidation.TestHelper;

namespace UnitTests.ValidatorTests;

public class UpdateCredentialPositionValidatorTests
{
    private readonly UpdateCredentialPositionValidator _updateCredentialPositionValidator = new();

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    public void PositionIsPositiveNumber_ShouldValidate(int position)
    {
        var positionRequest = new UpdateCredentialPositionRequest(1, position);

        var result = _updateCredentialPositionValidator.TestValidate(positionRequest);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-3)]
    public void PositionIsNotPositiveNumber_ShouldNotValidate(int position)
    {
        var positionRequest = new UpdateCredentialPositionRequest(1, position);

        var result = _updateCredentialPositionValidator.TestValidate(positionRequest);

        result.ShouldHaveValidationErrorFor(x => x.Position);
    }
}