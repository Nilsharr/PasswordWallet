using PasswordWallet.Server.Repositories;
using PasswordWallet.Server.Utils;
using PasswordWallet.Shared.Dtos;
using IMapper = AutoMapper.IMapper;

namespace PasswordWallet.Server.Endpoints.Account.Credentials;

public class GetCredentialEndpoint : Endpoint<IdRequestDto, CredentialsDto>
{
    private readonly ICredentialsRepository _credentialsRepository;
    private readonly IMapper _mapper;

    public GetCredentialEndpoint(ICredentialsRepository credentialsRepository, IMapper mapper)
    {
        _credentialsRepository = credentialsRepository;
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

        var credentials = await _credentialsRepository.Get(req.Id, ct);
        await SendAsync(_mapper.Map<CredentialsDto>(credentials), cancellation: ct);
    }
}