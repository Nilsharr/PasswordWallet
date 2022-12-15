using FluentValidation.Results;
using PasswordWallet.Server.Services;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.Server.Endpoints.Account.Login;

public class PreLoginCheck<TRequest> : IPreProcessor<TRequest>
{
    public async Task PreProcessAsync(TRequest req, HttpContext ctx, List<ValidationFailure> failures,
        CancellationToken ct)
    {
        var auditService = ctx.Resolve<ILoginAuditService>();
        var ip = ctx.Connection.RemoteIpAddress?.MapToIPv4();
        if (ip is not null)
        {
            var blockade = await auditService.GetIpAddressBlockadeStatus(ip, ct);
            if (blockade.permanentlyLocked)
            {
                await ctx.Response.SendForbiddenAsync(ct);
                return;
            }

            if (blockade.temporaryLock > DateTime.UtcNow)
            {
                await ctx.Response.SendForbiddenAsync(ct);
                return;
            }
        }

        // https://github.com/FastEndpoints/FastEndpoints/issues/230
        if (req is LoginRequestDto loginRequest)
        {
            var blockadeTime = await auditService.GetAccountBlockadeStatus(loginRequest.Login, ct);
            if (blockadeTime > DateTime.UtcNow)
            {
                //failures.Add(new("test", "test"));
                await ctx.Response.SendForbiddenAsync(ct);
            }
        }
    }
}