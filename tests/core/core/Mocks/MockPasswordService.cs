using System;

using Shipstone.Extensions.Identity;

namespace Shipstone.Authenticator.Api.CoreTest.Mocks;

internal sealed class MockPasswordService : IPasswordService
{
    internal Func<String, String> _hashFunc;
    internal Action<String> _validateAction;
    internal Func<String, String, bool> _verifyFunc;

    public MockPasswordService()
    {
        this._hashFunc = _ => throw new NotImplementedException();
        this._validateAction = _ => throw new NotImplementedException();
        this._verifyFunc = (_, _) => throw new NotImplementedException();
    }

    String IPasswordService.Hash(String password) => this._hashFunc(password);

    void IPasswordService.Validate(String password) =>
        this._validateAction(password);

    bool IPasswordService.Verify(String passwordHash, String password) =>
        this._verifyFunc(passwordHash, password);
}
