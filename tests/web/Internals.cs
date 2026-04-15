using Xunit;

using Shipstone.Authenticator.Api.Core;

namespace Shipstone.Authenticator.Api.WebTest;

internal static class Internals
{
    internal static void AssertNotAuthenticated(this IClaimsService claims)
    {
        Assert.Throws<UnauthorizedException>(() => claims.EmailAddress);
        Assert.Throws<UnauthorizedException>(() => claims.Id);
    }
}
