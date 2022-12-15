using FastEndpoints.Security;
using PasswordWallet.Server.Repositories;
using PasswordWallet.Shared.Dtos;
using Constants = PasswordWallet.Server.Utils.Constants;
using IMapper = AutoMapper.IMapper;

namespace PasswordWallet.Server.Endpoints.Account;

public class GetAccountSecurityDetailsEndpoint : Endpoint<EmptyRequest, AccountSecurityDetailsDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public GetAccountSecurityDetailsEndpoint(IAccountRepository accountRepository, IMapper mapper)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Get("/api/account/security-details");
        Claims(Constants.AccountIdClaim);
        Description(x => x.WithName("GetAccountSecurityDetails"));
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        long? accountId = long.TryParse(User.ClaimValue(Constants.AccountIdClaim), out var id) ? id : null;
        if (accountId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var ipAddresses =
            _mapper.Map<IList<LoginIpAddressDto>>(
                await _accountRepository.GetAccountSecurityDetails(accountId.Value, ct));
        var lastLogins = await _accountRepository.GetLastValidAndInvalidLogin(accountId.Value, ct);

        var response = new AccountSecurityDetailsDto
        {
            LoginRequest = ipAddresses, LastSuccessfulLogin = lastLogins.lastSuccessfulLogin,
            LastUnsuccessfulLogin = lastLogins.lastUnsuccessfulLogin
        };

        await SendAsync(response, cancellation: ct);
    }
}