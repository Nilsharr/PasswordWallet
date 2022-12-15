using PasswordWallet.Server.Repositories;
using PasswordWallet.Shared.Dtos;
using PasswordWallet.Server.Utils;

namespace PasswordWallet.Server.Endpoints.Account.Credentials;

public class DeleteCredentialEndpoint : Endpoint<IdRequestDto>
{
    private readonly ICredentialRepository _credentialRepository;

    public DeleteCredentialEndpoint(ICredentialRepository credentialRepository)
    {
        _credentialRepository = credentialRepository;
    }

    public override void Configure()
    {
        Delete("/api/account/credentials/{id}");
        Claims(Constants.AccountIdClaim);
        Description(x => x.WithName("DeleteCredential").Produces(200));
    }

    public override async Task HandleAsync(IdRequestDto req, CancellationToken ct)
    {
        if (req.AccountId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await _credentialRepository.Delete(req.Id, ct);
        await SendOkAsync(ct);
    }
}