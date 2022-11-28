using FluentValidation.TestHelper;
using PasswordWallet.Server.Validators;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.UnitTests.ValidatorsTests;

public class ChangePasswordValidatorTests
{
    private readonly ChangePasswordValidator _changePasswordValidator;

    public ChangePasswordValidatorTests()
    {
        _changePasswordValidator = new ChangePasswordValidator();
    }

    [Fact]
    public void ValidPasswordsThatMatch_ShouldValidate()
    {
        // Arrange
        var changePasswordRequestDto = new ChangePasswordRequestDto {Password = "test123", ConfirmPassword = "test123"};

        // Act
        var response = _changePasswordValidator.TestValidate(changePasswordRequestDto);

        // Assert
        response.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidPasswordsThatDoNotMatch_ShouldNotValidate()
    {
        // Arrange
        var changePasswordRequestDto = new ChangePasswordRequestDto
            {Password = "xyz987123", ConfirmPassword = "xyz123987"};

        // Act
        var response = _changePasswordValidator.TestValidate(changePasswordRequestDto);

        // Assert
        response.ShouldHaveValidationErrorFor(x => x.Password).Only();
    }

    [Fact]
    public void EmptyPasswords_ShouldNotValidate()
    {
        // Arrange
        var changePasswordRequestDto = new ChangePasswordRequestDto {Password = "", ConfirmPassword = ""};

        // Act
        var response = _changePasswordValidator.TestValidate(changePasswordRequestDto);

        // Assert
        response.ShouldHaveValidationErrorFor(x => x.Password);
        response.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
    }
}