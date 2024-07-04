using Api.Endpoints.v1.User.Register;
using Core.Interfaces.Repositories;
using FastEndpoints;
using FluentValidation.TestHelper;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace UnitTests.ValidatorTests;

public class RegisterValidatorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task ValidRegisterData_ShouldValidate()
    {
        const bool userWithUsernameExists = false;
        _unitOfWork.UserRepository.ExistsWithUsername(Arg.Any<string>()).Returns(userWithUsernameExists);
        var validator = Factory.CreateValidator<RegisterValidator>(s => { s.AddSingleton(_unitOfWork); });
        var registerRequest = new RegisterRequest("testUser", "Jc@$CDH9esf", "Jc@$CDH9esf");

        var result = await validator.TestValidateAsync(registerRequest);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    [InlineData("           ")]
    public async Task EmptyUsername_ShouldNotValidate(string? username)
    {
        const bool userWithUsernameExists = false;
        _unitOfWork.UserRepository.ExistsWithUsername(Arg.Any<string>()).Returns(userWithUsernameExists);
        var validator = Factory.CreateValidator<RegisterValidator>(s => { s.AddSingleton(_unitOfWork); });
        var registerRequest = new RegisterRequest(username!, "Jc@$CDH9esf", "Jc@$CDH9esf");

        var result = await validator.TestValidateAsync(registerRequest);

        result.ShouldHaveValidationErrorFor(x => x.Username).Only();
    }

    [Theory]
    [InlineData(31)]
    [InlineData(60)]
    [InlineData(80)]
    public async Task TooLongUsername_ShouldNotValidate(int stringLength)
    {
        const bool userWithUsernameExists = false;
        _unitOfWork.UserRepository.ExistsWithUsername(Arg.Any<string>()).Returns(userWithUsernameExists);
        var validator = Factory.CreateValidator<RegisterValidator>(s => { s.AddSingleton(_unitOfWork); });
        var registerRequest = new RegisterRequest(new string('s', stringLength), "Jc@$CDH9esf", "Jc@$CDH9esf");

        var result = await validator.TestValidateAsync(registerRequest);

        result.ShouldHaveValidationErrorFor(x => x.Username).Only();
    }


    [Fact]
    public async Task DuplicatedUsername_ShouldNotValidate()
    {
        const bool userWithUsernameExists = true;
        _unitOfWork.UserRepository.ExistsWithUsername(Arg.Any<string>()).Returns(userWithUsernameExists);
        var validator = Factory.CreateValidator<RegisterValidator>(s => { s.AddSingleton(_unitOfWork); });
        var registerRequest = new RegisterRequest("duplicatedUser", "Jc@$CDH9esf", "Jc@$CDH9esf");

        var result = await validator.TestValidateAsync(registerRequest);

        result.ShouldHaveValidationErrorFor(x => x.Username).Only();
    }
}