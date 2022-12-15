using PasswordWallet.Server.Services;
using PasswordWallet.Shared.Dtos;
using PasswordWallet.Server.Utils;
using IMapper = AutoMapper.IMapper;

namespace PasswordWallet.Server.Endpoints.Account.Credentials;

public class UpdateCredentialEndpoint : Endpoint<CredentialDto, CredentialDto>
{
    private readonly ICredentialService _credentialService;
    private readonly IMapper _mapper;

    public UpdateCredentialEndpoint(ICredentialService credentialService, IMapper mapper)
    {
        _credentialService = credentialService;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Put("/api/account/credentials/{id}");
        Claims(Constants.AccountIdClaim);
        Description(x => x.WithName("UpdateCredential"));
    }

    public override async Task HandleAsync(CredentialDto req, CancellationToken ct)
    {
        if (req.AccountId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var credential = await _credentialService.EncryptAndUpdateCredential(req.AccountId.Value,
            _mapper.Map<Entities.Credential>(req), ct);
        await SendAsync(_mapper.Map<CredentialDto>(credential), cancellation: ct);
    }
}