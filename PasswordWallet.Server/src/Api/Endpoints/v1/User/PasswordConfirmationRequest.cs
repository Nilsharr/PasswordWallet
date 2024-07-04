namespace Api.Endpoints.v1.User;

public record PasswordConfirmationRequest(string Password, string ConfirmPassword);