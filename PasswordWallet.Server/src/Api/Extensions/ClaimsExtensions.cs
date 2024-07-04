using System.Security.Claims;
using Core.Constants;

namespace Api.Extensions;

public static class ClaimsExtensions
{
    public static long? GetUserId(this ClaimsPrincipal claims) =>
        long.TryParse(claims.FindFirst(AuthenticationConstants.UserIdClaim)?.Value, out var id) ? id : null;
}