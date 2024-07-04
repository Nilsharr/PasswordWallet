using System.Net;
using Core.Interfaces.Repositories;
using FastEndpoints;
using FluentValidation.Results;

namespace Api.Endpoints.v1.User.Login;

public class PreLoginProcessor<TRequest> : IPreProcessor<TRequest>
{
    public async Task PreProcessAsync(IPreProcessorContext<TRequest> context, CancellationToken ct)
    {
        if (context.Request is LoginRequest loginRequest)
        {
            var unitOfWork = context.HttpContext.Resolve<IUnitOfWork>();
            var lockoutTime = await unitOfWork.UserRepository.GetLockoutTime(loginRequest.Username, ct);
            if (lockoutTime is not null)
            {
                var timeProvider = context.HttpContext.Resolve<TimeProvider>();
                var now = timeProvider.GetUtcNow();
                if (lockoutTime.Value > now)
                {
                    var timeDiff = lockoutTime.Value - now;
                    context.ValidationFailures.Add(new ValidationFailure("UserBlockade",
                        $"Too many incorrect log in attempts. Try again in {timeDiff.Minutes} min {timeDiff.Seconds} sec."));
                    await context.HttpContext.Response.SendErrorsAsync(context.ValidationFailures,
                        (int)HttpStatusCode.Forbidden, cancellation: ct);
                }
            }
        }
    }
}