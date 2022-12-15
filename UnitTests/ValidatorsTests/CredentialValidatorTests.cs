using FluentValidation.TestHelper;
using PasswordWallet.Server.Validators;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.UnitTests.ValidatorsTests;

public class CredentialValidatorTests
{
    private readonly CredentialValidator _credentialValidator;

    public CredentialValidatorTests()
    {
        _credentialValidator = new CredentialValidator();
    }

    [Theory]
    [InlineData("medklun", "xyz123", "xyz.com", "desc")]
    [InlineData("test234", "xyz123", "yzx.com", null)]
    [InlineData("plktgy", "xyz123", null, null)]
    [InlineData(null, "xyz123", null, null)]
    public void ValidCredential_ShouldValidate(string? login, string password, string? webAddress, string? description)
    {
        // Arrange
        var credentialDto = new CredentialDto
            {Login = login, Password = password, WebAddress = webAddress, Description = description};

        // Act
        var response = _credentialValidator.TestValidate(credentialDto);

        // Assert
        response.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyPassword_ShouldNotValidate()
    {
        // Arrange
        var credentialDto = new CredentialDto
            {Login = "test", Password = null!, WebAddress = "jkl.org", Description = null};

        // Act
        var response = _credentialValidator.TestValidate(credentialDto);

        // Assert
        response.ShouldHaveValidationErrorFor(x => x.Password).Only();
    }

    [Fact]
    public void TooLongLogin_ShouldNotValidate()
    {
        // Arrange
        var credentialDto = new CredentialDto
        {
            Login = string.Concat(Enumerable.Repeat("test123", 8)), Password = "741258963", WebAddress = null,
            Description = null
        };

        // Act
        var response = _credentialValidator.TestValidate(credentialDto);

        // Assert
        response.ShouldHaveValidationErrorFor(x => x.Login).Only();
    }

    [Fact]
    public void TooLongWebAddress_ShouldNotValidate()
    {
        // Arrange
        var credentialDto = new CredentialDto
        {
            Login = "test2", Password = "654321", WebAddress = string.Concat(Enumerable.Repeat("google.com", 30)),
            Description = null
        };

        // Act
        var response = _credentialValidator.TestValidate(credentialDto);

        // Assert
        response.ShouldHaveValidationErrorFor(x => x.WebAddress).Only();
    }

    [Fact]
    public void TooLongDescription_ShouldNotValidate()
    {
        // Arrange
        var credentialDto = new CredentialDto
        {
            Login = null, Password = "123456", WebAddress = "tbnm.com",
            Description = string.Concat(Enumerable.Repeat("Very long description", 15))
        };

        // Act
        var response = _credentialValidator.TestValidate(credentialDto);

        // Assert
        response.ShouldHaveValidationErrorFor(x => x.Description).Only();
    }
}