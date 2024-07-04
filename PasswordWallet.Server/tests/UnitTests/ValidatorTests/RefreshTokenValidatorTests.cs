using Api.Endpoints.v1.User.RefreshToken;
using FluentValidation.TestHelper;

namespace UnitTests.ValidatorTests;

public class RefreshTokenValidatorTests
{
    private readonly RefreshTokenValidator _tokenRequestValidator = new();

    [Theory]
    [InlineData("medklun", "ewfsdfwef")]
    [InlineData("test234", "fewfsdfsd")]
    [InlineData("plktgy", "wfewwefwf")]
    public void ValidTokenRequest_ShouldValidate(string accessToken, string refreshToken)
    {
        var tokenRequest = new RefreshTokenRequest(accessToken, refreshToken);

        var result = _tokenRequestValidator.TestValidate(tokenRequest);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    [InlineData("           ")]
    public void EmptyAccessToken_ShouldNotValidate(string? accessToken)
    {
        var tokenRequest = new RefreshTokenRequest(accessToken!, "wdeqasdasdw");

        var result = _tokenRequestValidator.TestValidate(tokenRequest);

        result.ShouldHaveValidationErrorFor(x => x.AccessToken).Only();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    [InlineData("           ")]
    public void EmptyRefreshToken_ShouldNotValidate(string? refreshToken)
    {
        var tokenRequest = new RefreshTokenRequest("xyzsadasdwqd", refreshToken!);

        var result = _tokenRequestValidator.TestValidate(tokenRequest);

        result.ShouldHaveValidationErrorFor(x => x.RefreshToken).Only();
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData(null, "")]
    [InlineData("       ", null)]
    public void EmptyTokenData_ShouldNotValidate(string? accessToken, string? refreshToken)
    {
        var tokenRequest = new RefreshTokenRequest(accessToken!, refreshToken!);

        var result = _tokenRequestValidator.TestValidate(tokenRequest);

        result.ShouldHaveValidationErrorFor(x => x.AccessToken);
        result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
    }
}