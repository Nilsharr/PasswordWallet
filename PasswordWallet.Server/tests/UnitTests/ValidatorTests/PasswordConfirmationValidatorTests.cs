using Api.Endpoints.v1.User;
using FluentValidation.TestHelper;

namespace UnitTests.ValidatorTests;

public class PasswordConfirmationValidatorTests
{
    private readonly PasswordConfirmationValidator _passwordConfirmationValidator = new();

    [Theory]
    [InlineData("4EfuNd^E2f")]
    [InlineData("v5#FdmvK5gh")]
    [InlineData("b786&Z9dAWECXD45")]
    public void ValidPasswords_ShouldValidate(string password)
    {
        var passwordConfirmation = new PasswordConfirmationRequest(password, password);

        var result = _passwordConfirmationValidator.TestValidate(passwordConfirmation);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    [InlineData("           ")]
    public void EmptyPassword_ShouldNotValidate(string? password)
    {
        var passwordConfirmation = new PasswordConfirmationRequest(password!, password!);

        var result = _passwordConfirmationValidator.TestValidate(passwordConfirmation);

        result.ShouldHaveValidationErrorFor(x => x.Password);
        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
    }

    [Theory]
    [InlineData("@cE)y6!g7")]
    [InlineData("2BMQxhC#")]
    [InlineData("n&Wg7sp")]
    public void TooShortPassword_ShouldNotValidate(string password)
    {
        var passwordConfirmation = new PasswordConfirmationRequest(password, password);

        var result = _passwordConfirmationValidator.TestValidate(passwordConfirmation);

        result.ShouldHaveValidationErrorFor(x => x.Password).Only();
    }

    [Theory]
    [InlineData("3KT&PBP$2Q")]
    [InlineData("7YJJMR@D%21")]
    [InlineData("XVTZMY#V34$")]
    public void PasswordWithoutLowerCaseLetter_ShouldNotValidate(string password)
    {
        var passwordConfirmation = new PasswordConfirmationRequest(password, password);

        var result = _passwordConfirmationValidator.TestValidate(passwordConfirmation);

        result.ShouldHaveValidationErrorFor(x => x.Password).Only();
    }

    [Theory]
    [InlineData("dqrt*3dcd35")]
    [InlineData("ofkd*l2pbn4554")]
    [InlineData("9l@qfbcb567ty")]
    public void PasswordWithoutCapitalLetter_ShouldNotValidate(string password)
    {
        var passwordConfirmation = new PasswordConfirmationRequest(password, password);

        var result = _passwordConfirmationValidator.TestValidate(passwordConfirmation);

        result.ShouldHaveValidationErrorFor(x => x.Password).Only();
    }

    [Theory]
    [InlineData("e*fDFsf$uP")]
    [InlineData("tqd)agJ*%G")]
    [InlineData("sXm%nAH&kU&")]
    public void PasswordWithoutNumber_ShouldNotValidate(string password)
    {
        var passwordConfirmation = new PasswordConfirmationRequest(password, password);

        var result = _passwordConfirmationValidator.TestValidate(passwordConfirmation);

        result.ShouldHaveValidationErrorFor(x => x.Password).Only();
    }

    [Theory]
    [InlineData("TGy4Zty55K")]
    [InlineData("5Grth78wQX")]
    [InlineData("bqV2Krty3q6")]
    public void PasswordWithoutSpecialCharacter_ShouldNotValidate(string password)
    {
        var passwordConfirmation = new PasswordConfirmationRequest(password, password);

        var result = _passwordConfirmationValidator.TestValidate(passwordConfirmation);

        result.ShouldHaveValidationErrorFor(x => x.Password).Only();
    }

    [Theory]
    [InlineData("zwPP%rY6rt", "zwPP%rY655")]
    [InlineData("@RR3@DJ!tk", "@RR3@DJ!mm")]
    [InlineData("!e9o*hpE123", "!e9o*hpE321")]
    public void NotMatchingPasswords_ShouldNotValidate(string password, string confirmPassword)
    {
        var passwordConfirmation = new PasswordConfirmationRequest(password, confirmPassword);

        var result = _passwordConfirmationValidator.TestValidate(passwordConfirmation);

        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword).Only();
    }
}