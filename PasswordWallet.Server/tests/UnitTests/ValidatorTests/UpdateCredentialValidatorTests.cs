using Api.Endpoints.v1.Credentials.UpdateCredential;
using FluentValidation.TestHelper;

namespace UnitTests.ValidatorTests;

public class UpdateCredentialValidatorTests
{
    private readonly UpdateCredentialValidator _updateCredentialValidator = new();

    [Theory]
    [InlineData("medklun", "xyz123", "xyz.com", "desc")]
    [InlineData("test234", "xyz123", "yzx.com", null)]
    [InlineData("plktgy", "xyz123", null, null)]
    [InlineData(null, "xyz123", null, null)]
    [InlineData(null, null, null, null)]
    public void ValidCredential_ShouldValidate(string? username, string? password, string? webAddress,
        string? description)
    {
        var credentialRequest = new UpdateCredentialRequest(1, username, password, webAddress, description);

        var result = _updateCredentialValidator.TestValidate(credentialRequest);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(61)]
    [InlineData(90)]
    [InlineData(120)]
    public void TooLongUsername_ShouldNotValidate(int stringLength)
    {
        var credentialRequest = new UpdateCredentialRequest(1, new string('a', stringLength), "741258963");

        var result = _updateCredentialValidator.TestValidate(credentialRequest);

        result.ShouldHaveValidationErrorFor(x => x.Username).Only();
    }

    [Theory]
    [InlineData(257)]
    [InlineData(300)]
    [InlineData(500)]
    public void TooLongWebAddress_ShouldNotValidate(int stringLength)
    {
        var credentialRequest = new UpdateCredentialRequest(1, "test2", "654321", new string('g', stringLength));

        var result = _updateCredentialValidator.TestValidate(credentialRequest);

        result.ShouldHaveValidationErrorFor(x => x.WebAddress).Only();
    }

    [Theory]
    [InlineData(513)]
    [InlineData(550)]
    [InlineData(600)]
    public void TooLongDescription_ShouldNotValidate(int stringLength)
    {
        var credentialRequest =
            new UpdateCredentialRequest(1, null, "123456", "tbnm.com", new string('d', stringLength));

        var result = _updateCredentialValidator.TestValidate(credentialRequest);

        result.ShouldHaveValidationErrorFor(x => x.Description).Only();
    }
}