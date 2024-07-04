using Api.Endpoints.v1.User.Login;
using FluentValidation.TestHelper;

namespace UnitTests.ValidatorTests;

public class LoginValidatorTests
{
    private readonly LoginValidator _loginValidator = new();

    [Fact]
    public void ValidLoginData_ShouldValidate()
    {
        var loginRequest = new LoginRequest("testUser", "testPassword");

        var result = _loginValidator.TestValidate(loginRequest);

        result.ShouldNotHaveAnyValidationErrors();
    }


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    [InlineData("           ")]
    public void EmptyUsername_ShouldNotValidate(string? username)
    {
        var loginRequest = new LoginRequest(username!, "pass123");

        var result = _loginValidator.TestValidate(loginRequest);

        result.ShouldHaveValidationErrorFor(x => x.Username).Only();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    [InlineData("           ")]
    public void EmptyPassword_ShouldNotValidate(string? password)
    {
        var loginRequest = new LoginRequest("log123", password!);

        var result = _loginValidator.TestValidate(loginRequest);

        result.ShouldHaveValidationErrorFor(x => x.Password).Only();
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData(null, "")]
    [InlineData("       ", null)]
    public void EmptyLoginData_ShouldNotValidate(string? username, string? password)
    {
        var loginRequest = new LoginRequest(username!, password!);

        var result = _loginValidator.TestValidate(loginRequest);

        result.ShouldHaveValidationErrorFor(x => x.Username);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}