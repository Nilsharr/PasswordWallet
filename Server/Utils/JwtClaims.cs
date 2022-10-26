using System.Security.Claims;

namespace PasswordWallet.Server.Utils;

public class JwtClaims
{
    private static string? GetClaimValue(ClaimsIdentity? claimsIdentity, string key)
    {
        return claimsIdentity?.Claims.Where(x => x.Type == key).Select(x => x.Value)
            .FirstOrDefault();
    }

    public static int? GetAccountIdFromClaims(ClaimsIdentity? claimsIdentity)
    {
        var isParsed = int.TryParse(GetClaimValue(claimsIdentity, Constants.AccountIdClaim), out var accountId);
        return isParsed ? accountId : null;
    }
}