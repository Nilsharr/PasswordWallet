using PasswordWallet.Server.Repositories;
using PasswordWallet.Server.Utils;
using PasswordWallet.Shared.Dtos;
using IMapper = AutoMapper.IMapper;

namespace PasswordWallet.Server.Endpoints.Account.Credentials;

public class GetCredentialEndpoint : Endpoint<IdRequestDto, CredentialDto>
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly IMapper _mapper;

    public GetCredentialEndpoint(ICredentialRepository credentialRepository, IMapper mapper)
    {
        _credentialRepository = credentialRepository;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Get("/api/account/credentials/{id}");
        Claims(Constants.AccountIdClaim);
        Description(x => x.WithName("GetCredential"));
    }

    public override async Task HandleAsync(IdRequestDto req, CancellationToken ct)
    {
        if (req.AccountId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        //todo not found
        var credential = await _credentialRepository.Get(req.Id, ct);
        await SendAsync(_mapper.Map<CredentialDto>(credential), cancellation: ct);
    }
}