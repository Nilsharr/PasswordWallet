using FluentValidation.TestHelper;
using PasswordWallet.Server.Validators;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.UnitTests.ValidatorsTests;

public class LoginValidatorTests
{
    private readonly LoginValidator _loginValidator;

    public LoginValidatorTests()
    {
        _loginValidator = new LoginValidator();
    }

    [Fact]
    public void ValidLoginData_ShouldValidate()
    {
        // Arrange
        var loginRequestDto = new LoginRequestDto
            {Login = "testLogin", Password = "testPassword"};

        // Act
        var response = _loginValidator.TestValidate(loginRequestDto);

        // Assert
        response.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void TooLongLogin_ShouldNotValidate()
    {
        // Arrange
        var loginRequestDto = new LoginRequestDto
            {Login = string.Concat(Enumerable.Repeat("qwerty", 7)), Password = "vbnmghf7452"};

        // Act
        var response = _loginValidator.TestValidate(loginRequestDto);

        // Assert
        response.ShouldHaveValidationErrorFor(x => x.Login).Only();
    }

    [Fact]
    public void EmptyLogin_ShouldNotValidate()
    {
        // Arrange
        var loginRequestDto = new LoginRequestDto {Login = null!, Password = "pass123"};

        // Act
        var response = _loginValidator.TestValidate(loginRequestDto);

        // Assert
        response.ShouldHaveValidationErrorFor(x => x.Login).Only();
    }

    [Fact]
    public void EmptyPassword_ShouldNotValidate()
    {
        // Arrange
        var loginRequestDto = new LoginRequestDto {Login = "log123", Password = null!};

        // Act
        var response = _loginValidator.TestValidate(loginRequestDto);

        // Assert
        response.ShouldHaveValidationErrorFor(x => x.Password).Only();
    }

    [Fact]
    public void EmptyLoginData_ShouldNotValidate()
    {
        // Arrange
        var loginRequestDto = new LoginRequestDto {Login = null!, Password = null!};

        // Act
        var response = _loginValidator.TestValidate(loginRequestDto);

        // Assert
        response.ShouldHaveValidationErrorFor(x => x.Login);
        response.ShouldHaveValidationErrorFor(x => x.Password);
    }
}