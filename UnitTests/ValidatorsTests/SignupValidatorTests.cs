using FluentValidation.TestHelper;
using PasswordWallet.Server.Validators;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.UnitTests.ValidatorsTests;

public class SignupValidatorTests
{
    private readonly SignupValidator _signupValidator;

    public SignupValidatorTests()
    {
        _signupValidator = new SignupValidator();
    }

    [Fact]
    public void ValidSignupData_ShouldValidate()
    {
        // Arrange
        var signupRequestDto = new SignupRequestDto
            {Login = "testLogin", Password = "testPassword", ConfirmPassword = "testPassword"};

        // Act
        var response = _signupValidator.TestValidate(signupRequestDto);

        // Assert
        response.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void TooLongLogin_ShouldNotValidate()
    {
        // Arrange
        var signupRequestDto = new SignupRequestDto
        {
            Login = string.Concat(Enumerable.Repeat("mnbvcx", 7)), Password = "vbnmghf7452",
            ConfirmPassword = "vbnmghf7452"
        };

        // Act
        var response = _signupValidator.TestValidate(signupRequestDto);

        // Assert
        response.ShouldHaveValidationErrorFor(x => x.Login).Only();
    }

    [Fact]
    public void ValidPasswordsThatDoNotMatch_ShouldNotValidate()
    {
        // Arrange
        var signupRequestDto = new SignupRequestDto
            {Login = "mondefgh", Password = "xyz987123", ConfirmPassword = "xyz123987"};

        // Act
        var response = _signupValidator.TestValidate(signupRequestDto);

        // Assert
        response.ShouldHaveValidationErrorFor(x => x.Password).Only();
    }

    [Fact]
    public void EmptyLogin_ShouldNotValidate()
    {
        // Arrange
        var signupRequestDto = new SignupRequestDto {Login = null!, Password = "pass123", ConfirmPassword = "pass123"};

        // Act
        var response = _signupValidator.TestValidate(signupRequestDto);

        // Assert
        response.ShouldHaveValidationErrorFor(x => x.Login).Only();
    }

    [Fact]
    public void EmptyPasswords_ShouldNotValidate()
    {
        // Arrange
        var signupRequestDto = new SignupRequestDto {Login = "qwerty", Password = null!, ConfirmPassword = null!};

        // Act
        var response = _signupValidator.TestValidate(signupRequestDto);

        // Assert
        response.ShouldHaveValidationErrorFor(x => x.Password);
        response.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
    }

    [Fact]
    public void EmptySignupData_ShouldNotValidate()
    {
        // Arrange
        var signupRequestDto = new SignupRequestDto {Login = null!, Password = null!, ConfirmPassword = null!};

        // Act
        var response = _signupValidator.TestValidate(signupRequestDto);

        // Assert
        response.ShouldHaveValidationErrorFor(x => x.Login);
        response.ShouldHaveValidationErrorFor(x => x.Password);
        response.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
    }
}