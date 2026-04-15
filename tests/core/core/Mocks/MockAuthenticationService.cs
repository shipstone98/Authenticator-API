using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Authenticator.Api.Core.Accounts;
using Shipstone.Authenticator.Api.Infrastructure.Authentication;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.CoreTest.Mocks;

internal sealed class MockAuthenticationService : IAuthenticationService
{
    internal Func<String, UserEntity, DateTime, IAuthenticateResult> _authenticateAction;
    internal Action<UserEntity, DateTime> _generateOtpAction;
    internal Func<String, (String, Guid)> _getPropertiesFunc;

    public MockAuthenticationService()
    {
        this._authenticateAction = (_, _, _) =>
            throw new NotImplementedException();

        this._generateOtpAction = (_, _) =>
            throw new NotImplementedException();

        this._getPropertiesFunc = _ => throw new NotImplementedException();
    }

    Task<IAuthenticateResult> IAuthenticationService.AuthenticateAsync(
        String audience,
        UserEntity user,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        IAuthenticateResult result =
            this._authenticateAction(audience, user, now);

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

    Task<(String, Guid)> IAuthenticationService.GetPropertiesAsync(
        String token,
        CancellationToken cancellationToken
    )
    {
        (String, Guid) result = this._getPropertiesFunc(token);
        return Task.FromResult(result);
    }
}
