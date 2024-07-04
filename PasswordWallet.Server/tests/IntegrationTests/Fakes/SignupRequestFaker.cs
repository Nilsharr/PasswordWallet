using Api.Endpoints.v1.User.Register;
using Bogus;

namespace IntegrationTests.Fakes;

public sealed class RegisterRequestFaker : Faker<RegisterRequest>
{
    private const string StrongPassword = "fkLM36M#5#";

    public RegisterRequestFaker(string? password = null)
    {
        password ??= StrongPassword;
        CustomInstantiator(f => new RegisterRequest(f.Internet.UserName(), password, password));
    }
}