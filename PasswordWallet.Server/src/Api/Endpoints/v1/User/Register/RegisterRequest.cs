namespace Api.Endpoints.v1.User.Register;

public record RegisterRequest(string Username, string Password, string ConfirmPassword)
    : PasswordConfirmationRequest(Password, ConfirmPassword);