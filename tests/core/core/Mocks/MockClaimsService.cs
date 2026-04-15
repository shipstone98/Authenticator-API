using System;

using Shipstone.Authenticator.Api.Core;

namespace Shipstone.Authenticator.Api.CoreTest.Mocks;

internal sealed class MockClaimsService : IClaimsService
{
    internal Func<String> _emailAddressFunc;
    internal Func<Guid> _idFunc;

    String IClaimsService.EmailAddress => this._emailAddressFunc();
    Guid IClaimsService.Id => this._idFunc();

    public MockClaimsService()
    {
        this._emailAddressFunc = () => throw new NotImplementedException();
        this._idFunc = () => throw new NotImplementedException();
    }
}
