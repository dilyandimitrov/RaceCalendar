using System.Security.Claims;

namespace RaceCalendar.Api.Utils;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return GetClaim(user, "Id");
    }

    public static string GetCurrentUserEmail(this ClaimsPrincipal user)
    {
        return GetClaim(user, "emailaddress");
    }

    public static string GetUniqueName(this ClaimsPrincipal user)
    {
        return GetClaim(user, "name");
    }

    private static string GetClaim(ClaimsPrincipal user, string claimName)
    {
        var claims = user.Claims.ToList();
        var claimValue = claims?.FirstOrDefault(x => x.Type.EndsWith(claimName, StringComparison.OrdinalIgnoreCase))?.Value;

        if (claimValue is null)
        {
            throw new InvalidOperationException($"Claim with value {claimName} does not exist for the user.");
        }

        return claimValue;
    }
}
