using Api.Endpoints.v1.User.GetLoginHistory;
using FluentValidation.TestHelper;

namespace UnitTests.ValidatorTests;

public class GetLoginHistoryValidatorTests
{
    private readonly GetLoginHistoryValidator _getLoginHistoryValidator = new();

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 20)]
    [InlineData(2, 20)]
    [InlineData(10, 40)]
    public void ValidPaginationRequest_ShouldValidate(int pageNumber, int pageSize)
    {
        var getLoginHistoryRequest = new GetLoginHistoryRequest(pageNumber, pageSize);

        var result = _getLoginHistoryValidator.TestValidate(getLoginHistoryRequest);

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
        var getLoginHistoryRequest = new GetLoginHistoryRequest(pageNumber, 30);

        var result = _getLoginHistoryValidator.TestValidate(getLoginHistoryRequest);

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
        var getLoginHistoryRequest = new GetLoginHistoryRequest(10, pageSize);

        var result = _getLoginHistoryValidator.TestValidate(getLoginHistoryRequest);

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
        var getLoginHistoryRequest = new GetLoginHistoryRequest(10, pageSize);

        var result = _getLoginHistoryValidator.TestValidate(getLoginHistoryRequest);

        result.ShouldHaveValidationErrorFor(x => x.PageSize).Only();
    }
}