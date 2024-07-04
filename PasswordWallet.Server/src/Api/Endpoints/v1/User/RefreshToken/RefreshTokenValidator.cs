using FastEndpoints;
using FluentValidation;

namespace Api.Endpoints.v1.User.RefreshToken;

public class RefreshTokenValidator : Validator<RefreshTokenRequest>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Access Token is required.");
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Refresh Token is required.");
    }
}