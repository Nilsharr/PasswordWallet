using Api.Endpoints.v1.Credentials.GetCredentials;
using FluentValidation.TestHelper;

namespace UnitTests.ValidatorTests;

public class GetCredentialsValidatorTests
{
    private readonly GetCredentialsValidator _getCredentialsValidator = new();

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 20)]
    [InlineData(5, 20)]
    [InlineData(10, 40)]
    public void ValidPaginationRequest_ShouldValidate(int pageNumber, int pageSize)
    {
        var getCredentialsRequest = new GetCredentialsRequest(Guid.NewGuid(), pageNumber, pageSize);

        var result = _getCredentialsValidator.TestValidate(getCredentialsRequest);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    [InlineData(-10)]
    [InlineData(-20)]
    public void PageNumberIsNotPositiveNumber_ShouldNotValidate(int pageNumber)
    {
        var getCredentialsRequest = new GetCredentialsRequest(Guid.NewGuid(), pageNumber, 30);

        var result = _getCredentialsValidator.TestValidate(getCredentialsRequest);

        result.ShouldHaveValidationErrorFor(x => x.PageNumber).Only();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    [InlineData(-10)]
    [InlineData(-20)]
    public void PageSizeIsNotPositiveNumber_ShouldNotValidate(int pageSize)
    {
        var getCredentialsRequest = new GetCredentialsRequest(Guid.NewGuid(), 10, pageSize);

        var result = _getCredentialsValidator.TestValidate(getCredentialsRequest);

        result.ShouldHaveValidationErrorFor(x => x.PageSize).Only();
    }

    [Theory]
    [InlineData(101)]
    [InlineData(150)]
    [InlineData(200)]
    [InlineData(250)]
    [InlineData(500)]
    public void PageSizeIsTooBig_ShouldNotValidate(int pageSize)
    {
        var getCredentialsRequest = new GetCredentialsRequest(Guid.NewGuid(), 10, pageSize);

        var result = _getCredentialsValidator.TestValidate(getCredentialsRequest);

        result.ShouldHaveValidationErrorFor(x => x.PageSize).Only();
    }
}