using PasswordWallet.Server.Repositories;
using PasswordWallet.Shared.Dtos;
using Constants = PasswordWallet.Server.Utils.Constants;

namespace PasswordWallet.Server.Endpoints.Account;

public class UnblockIpAddressEndpoint : Endpoint<IdRequestDto, EmptyResponse>
{
    private readonly IAccountRepository _accountRepository;

    public UnblockIpAddressEndpoint(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public override void Configure()
    {
        // TODO why its not working on post?
        Get("/api/account/unblock-ip/{id}");
        Claims(Constants.AccountIdClaim);
        Description(x => x.WithName("UnblockIpAddress"));
    }

    public override async Task HandleAsync(IdRequestDto req, CancellationToken ct)
    {
        if (req.AccountId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await _accountRepository.UnblockIpAddress(req.Id, ct);
        await SendOkAsync(ct);
    }
}