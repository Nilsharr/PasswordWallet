using PasswordWallet.Server.Services;
using PasswordWallet.Shared.Dtos;
using PasswordWallet.Server.Utils;
using IMapper = AutoMapper.IMapper;

namespace PasswordWallet.Server.Endpoints.Account.Credentials;

public class AddCredentialEndpoint : Endpoint<CredentialsDto, CredentialsDto>
{
    private readonly ICredentialsService _credentialsService;
    private readonly IMapper _mapper;

    public AddCredentialEndpoint(ICredentialsService credentialsService, IMapper mapper)
    {
        _credentialsService = credentialsService;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Post("/api/account/credentials");
        Claims(Constants.AccountIdClaim);
        Description(x => x.WithName("AddCredential").Produces<CredentialsDto>(201));
    }

    public override async Task HandleAsync(CredentialsDto req, CancellationToken ct)
    {
        if (req.AccountId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var credential =
            await _credentialsService.EncryptAndSaveCredential(req.AccountId.Value,
                _mapper.Map<Entities.Credentials>(req), ct);
        await SendCreatedAtAsync("GetCredential", new {credential.Id}, _mapper.Map<CredentialsDto>(credential),
            cancellation: ct);
    }
}