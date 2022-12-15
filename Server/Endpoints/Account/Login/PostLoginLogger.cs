using FluentValidation.Results;
using PasswordWallet.Server.Services;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.Server.Endpoints.Account.Login;

public class PostLoginLogger<TRequest, TResponse> : IPostProcessor<TRequest, TResponse>
{
    public async Task PostProcessAsync(TRequest req, TResponse res, HttpContext context,
        IReadOnlyCollection<ValidationFailure> failures, CancellationToken ct)
    {
        var auditService = context.Resolve<ILoginAuditService>();
        var ip = context.Connection.RemoteIpAddress?.MapToIPv4();
        if (req is LoginRequestDto loginRequest && ip is not null)
        {
            await auditService.RegisterLoginAttempt(ip, loginRequest.Login, res is AuthorizationResponseDto, ct);
        }
    }
}