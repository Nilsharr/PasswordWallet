using PasswordWallet.Server.Services;
using PasswordWallet.Shared.Dtos;
using PasswordWallet.Server.Utils;
using IMapper = AutoMapper.IMapper;

namespace PasswordWallet.Server.Endpoints.Account.Credentials;

public class AddCredentialEndpoint : Endpoint<CredentialDto, CredentialDto>
{
    private readonly ICredentialService _credentialService;
    private readonly IMapper _mapper;

    public AddCredentialEndpoint(ICredentialService credentialService, IMapper mapper)
    {
        _credentialService = credentialService;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Post("/api/account/credentials");
        Claims(Constants.AccountIdClaim);
        Description(x => x.WithName("AddCredential").Produces<CredentialDto>(201));
    }

    public override async Task HandleAsync(CredentialDto req, CancellationToken ct)
    {
        if (req.AccountId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var credential =
            await _credentialService.EncryptAndSaveCredential(req.AccountId.Value,
                _mapper.Map<Entities.Credential>(req), ct);
        await SendCreatedAtAsync("GetCredential", new {credential.Id}, _mapper.Map<CredentialDto>(credential),
            cancellation: ct);
    }
}