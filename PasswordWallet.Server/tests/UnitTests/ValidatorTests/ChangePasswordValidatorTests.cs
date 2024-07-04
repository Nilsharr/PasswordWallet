using Api.Endpoints.v1.User.ChangePassword;
using FluentValidation.TestHelper;

namespace UnitTests.ValidatorTests;

public class ChangePasswordValidatorTests
{
    private readonly ChangePasswordValidator _changePasswordValidator = new();

    [Theory]
    [InlineData("K8VD%xkd14")]
    [InlineData("aKYb8&LXu9^")]
    [InlineData("Q8&v6%)Hr5l1")]
    public void NotEmptyCurrentPassword_ShouldValidate(string currentPassword)
    {
        const string newPassword = "v5#FdmvK5gh";
        var changePasswordRequest = new ChangePasswordRequest(currentPassword, newPassword, newPassword);

        var result = _changePasswordValidator.TestValidate(changePasswordRequest);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    [InlineData("           ")]
    public void EmptyCurrentPassword_ShouldNotValidate(string? currentPassword)
    {
        const string newPassword = "v5#FdmvK5gh";
        var changePasswordRequest = new ChangePasswordRequest(currentPassword!, newPassword, newPassword);

        var result = _changePasswordValidator.TestValidate(changePasswordRequest);

        result.ShouldHaveValidationErrorFor(x => x.CurrentPassword);
    }
}