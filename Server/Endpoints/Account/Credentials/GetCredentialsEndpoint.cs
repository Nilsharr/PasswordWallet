using FastEndpoints.Security;
using PasswordWallet.Server.Repositories;
using PasswordWallet.Shared.Dtos;
using Constants = PasswordWallet.Server.Utils.Constants;
using IMapper = AutoMapper.IMapper;

namespace PasswordWallet.Server.Endpoints.Account.Credentials;

// TODO: add pagination
public class GetCredentialsEndpoint : Endpoint<EmptyRequest, IList<CredentialDto>>
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly IMapper _mapper;

    public GetCredentialsEndpoint(ICredentialRepository credentialRepository, IMapper mapper)
    {
        _credentialRepository = credentialRepository;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Get("/api/account/credentials");
        Claims(Constants.AccountIdClaim);
        Description(x => x.WithName("GetCredentials"));
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        long? accountId = long.TryParse(User.ClaimValue(Constants.AccountIdClaim), out var id) ? id : null;
        if (accountId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var credentials = await _credentialRepository.GetAll(accountId.Value, ct);
        await SendAsync(_mapper.Map<IList<CredentialDto>>(credentials), cancellation: ct);
    }
}