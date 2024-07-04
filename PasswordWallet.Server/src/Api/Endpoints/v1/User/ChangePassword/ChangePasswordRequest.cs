using FastEndpoints;

namespace Api.Endpoints.v1.User.ChangePassword;

public record ChangePasswordRequest(
    string CurrentPassword,
    string Password,
    string ConfirmPassword,
    [property: FromClaim] long UserId = 0)
    : PasswordConfirmationRequest(Password, ConfirmPassword);