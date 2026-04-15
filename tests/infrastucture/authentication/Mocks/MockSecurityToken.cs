using System;
using Microsoft.IdentityModel.Tokens;

namespace Shipstone.Authenticator.Api.Infrastructure.AuthenticationTest.Mocks;

internal sealed class MockSecurityToken : SecurityToken
{
    public sealed override String Id => throw new NotImplementedException();

    public sealed override String Issuer =>
        throw new NotImplementedException();

    public sealed override SecurityKey SecurityKey =>
        throw new NotImplementedException();

    public sealed override SecurityKey SigningKey
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public sealed override DateTime ValidFrom =>
        throw new NotImplementedException();

    public sealed override DateTime ValidTo =>
        throw new NotImplementedException();

    public sealed override bool Equals(Object? obj) =>
        throw new NotImplementedException();

    public sealed override int GetHashCode() =>
        throw new NotImplementedException();

    public sealed override String? ToString() =>
        throw new NotImplementedException();

    public sealed override String UnsafeToString() =>
        throw new NotImplementedException();
}
