using PasswordWallet.Server.Services;
using PasswordWallet.Shared.Dtos;
using PasswordWallet.Server.Utils;
using IMapper = AutoMapper.IMapper;

namespace PasswordWallet.Server.Endpoints.Account.Credentials;

public class UpdateCredentialEndpoint : Endpoint<CredentialsDto, CredentialsDto>
{
    private readonly ICredentialsService _credentialsService;
    private readonly IMapper _mapper;

    public UpdateCredentialEndpoint(ICredentialsService credentialsService, IMapper mapper)
    {
        _credentialsService = credentialsService;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Put("/api/account/credentials/{id}");
        Claims(Constants.AccountIdClaim);
        Description(x => x.WithName("UpdateCredential"));
    }

    public override async Task HandleAsync(CredentialsDto req, CancellationToken ct)
    {
        if (req.AccountId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var credential = await _credentialsService.EncryptAndUpdateCredential(req.AccountId.Value,
            _mapper.Map<Entities.Credentials>(req), ct);
        await SendAsync(_mapper.Map<CredentialsDto>(credential), cancellation: ct);
    }
}