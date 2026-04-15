using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Authenticator.Api.Core.Accounts;
using Shipstone.Authenticator.Api.Infrastructure.Authentication;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.CoreTest.Mocks;

internal sealed class MockAuthenticationService : IAuthenticationService
{
    internal Func<UserEntity, DateTime, IAuthenticateResult> _authenticateAction;
    internal Action<UserEntity, DateTime> _generateOtpAction;
    internal Func<String, Guid> _getIdFunc;

    public MockAuthenticationService()
    {
        this._authenticateAction = (_, _) =>
            throw new NotImplementedException();

        this._generateOtpAction = (_, _) =>
            throw new NotImplementedException();

        this._getIdFunc = _ => throw new NotImplementedException();
    }

    Task<IAuthenticateResult> IAuthenticationService.AuthenticateAsync(
        UserEntity user,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        IAuthenticateResult result = this._authenticateAction(user, now);
        return Task.FromResult(result);
    }

    Task IAuthenticationService.GenerateOtpAsync(
        UserEntity user,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        this._generateOtpAction(user, now);
        return Task.CompletedTask;
    }

    Guid IAuthenticationService.GetId(String token) => this._getIdFunc(token);
}
