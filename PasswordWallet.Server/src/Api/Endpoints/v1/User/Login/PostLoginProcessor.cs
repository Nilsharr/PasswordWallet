using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Models;
using FastEndpoints;
using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Api.Endpoints.v1.User.Login;

public class PostLoginProcessor<TRequest, TResponse> : IPostProcessor<TRequest, TResponse>
{
    public async Task PostProcessAsync(IPostProcessorContext<TRequest, TResponse> context, CancellationToken ct)
    {
        if (context.Request is LoginRequest loginRequest)
        {
            var unitOfWork = context.HttpContext.Resolve<IUnitOfWork>();
            var user = await unitOfWork.UserRepository.GetByUsername(loginRequest.Username, ct);
            if (user is null)
            {
                return;
            }

            var loginSecurityOptions = context.HttpContext.Resolve<IOptions<LoginSecurityOptions>>().Value;
            var correct = context.Response is AuthenticationResponse;
            var login = new LoginHistory
            {
                Correct = correct,
                IpAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserId = user.Id,
            };
            await unitOfWork.LoginHistoryRepository.Add(login, ct);

            user.SubsequentBadLogins = correct ? 0 : user.SubsequentBadLogins + 1;
            if (loginSecurityOptions.MaxFailedAccessAttempts > 0 &&
                user.SubsequentBadLogins == loginSecurityOptions.MaxFailedAccessAttempts)
            {
                var timeProvider = context.HttpContext.Resolve<TimeProvider>();
                user.LockoutTime = timeProvider.GetUtcNow().Add(loginSecurityOptions.LockoutTime);
                user.SubsequentBadLogins = 0;
            }

            await unitOfWork.SaveChangesAsync(ct);
        }
    }
}