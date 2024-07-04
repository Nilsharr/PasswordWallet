using FastEndpoints;
using FluentValidation;

namespace Api.Endpoints.v1.User.GetLoginHistory;

public class GetLoginHistoryValidator : Validator<GetLoginHistoryRequest>
{
    public GetLoginHistoryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("Must be be a positive page number.");
        RuleFor(x => x.PageSize).GreaterThan(0).WithMessage("Page size must be greater than 0.").LessThanOrEqualTo(100)
            .WithMessage("Page size can't exceed 100.");
    }
}