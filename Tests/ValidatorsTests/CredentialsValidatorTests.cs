using FluentValidation.TestHelper;
using PasswordWallet.Server.Validators;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.UnitTests.ValidatorsTests;

public class CredentialsValidatorTests
{
    private readonly CredentialsValidator _credentialsValidator;

    public CredentialsValidatorTests()
    {
        _credentialsValidator = new CredentialsValidator();
    }

    [Theory]
    [InlineData("medklun", "xyz123", "xyz.com", "desc")]
    [InlineData("test234", "xyz123", "yzx.com", null)]
    [InlineData("plktgy", "xyz123", null, null)]
    [InlineData(null, "xyz123", null, null)]
    public void ValidCredentials_ShouldValidate(string? login, string password, string? webAddress, string? description)
    {
        // Arrange
        var credentialsDto = new CredentialsDto
            {Login = login, Password = password, WebAddress = webAddress, Description = description};

        // Act
        var response = _credentialsValidator.TestValidate(credentialsDto);

        // Assert
        response.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyPassword_ShouldNotValidate()
    {
        // Arrange
        var credentialsDto = new CredentialsDto
            {Login = "test", Password = null!, WebAddress = "jkl.org", Description = null};

        // Act
        var response = _credentialsValidator.TestValidate(credentialsDto);

        // Assert
        response.ShouldHaveValidationErrorFor(x => x.Password).Only();
    }

    [Fact]
    public void TooLongLogin_ShouldNotValidate()
    {
        // Arrange
        var credentialsDto = new CredentialsDto
        {
            Login = string.Concat(Enumerable.Repeat("test123", 8)), Password = "741258963", WebAddress = null,
            Description = null
        };

        // Act
        var response = _credentialsValidator.TestValidate(credentialsDto);

        // Assert
        response.ShouldHaveValidationErrorFor(x => x.Login).Only();
    }

    [Fact]
    public void TooLongWebAddress_ShouldNotValidate()
    {
        // Arrange
        var credentialsDto = new CredentialsDto
        {
            Login = "test2", Password = "654321", WebAddress = string.Concat(Enumerable.Repeat("google.com", 30)),
            Description = null
        };

        // Act
        var response = _credentialsValidator.TestValidate(credentialsDto);

        // Assert
        response.ShouldHaveValidationErrorFor(x => x.WebAddress).Only();
    }

    [Fact]
    public void TooLongDescription_ShouldNotValidate()
    {
        // Arrange
        var credentialsDto = new CredentialsDto
        {
            Login = null, Password = "123456", WebAddress = "tbnm.com",
            Description = string.Concat(Enumerable.Repeat("Very long description", 15))
        };

        // Act
        var response = _credentialsValidator.TestValidate(credentialsDto);

        // Assert
        response.ShouldHaveValidationErrorFor(x => x.Description).Only();
    }
}