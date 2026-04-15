using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Authenticator.Api.Infrastructure.Entities;
using Shipstone.Authenticator.Api.Infrastructure.Mail;

namespace Shipstone.Authenticator.Api.CoreTest.Mocks;

internal sealed class MockMailService : IMailService
{
    internal Action<UserEntity, int> _sendOtpAction;
    internal Action<UserEntity, int> _sendPasswordResetAction;
    internal Action<UserEntity, int> _sendRegistrationAction;
    internal Action<String> _sendUnregistrationAction;

    public MockMailService()
    {
        this._sendOtpAction = (_, _) => throw new NotImplementedException();

        this._sendPasswordResetAction = (_, _) =>
            throw new NotImplementedException();

        this._sendRegistrationAction = (_, _) =>
            throw new NotImplementedException();

        this._sendUnregistrationAction = _ =>
            throw new NotImplementedException();
    }

    Task IMailService.SendOtpAsync(
        UserEntity user,
        int expiryMinutes,
        CancellationToken cancellationToken
    )
    {
        this._sendOtpAction(user, expiryMinutes);
        return Task.CompletedTask;
    }

    Task IMailService.SendPasswordResetAsync(
        UserEntity user,
        int expiryMinutes,
        CancellationToken cancellationToken
    )
    {
        this._sendPasswordResetAction(user, expiryMinutes);
        return Task.CompletedTask;
    }

    Task IMailService.SendRegistrationAsync(
        UserEntity user,
        int expiryMinutes,
        CancellationToken cancellationToken
    )
    {
        this._sendRegistrationAction(user, expiryMinutes);
        return Task.CompletedTask;
    }

    Task IMailService.SendUnregistrationAsync(
        String emailAddress,
        CancellationToken cancellationToken
    )
    {
        this._sendUnregistrationAction(emailAddress);
        return Task.CompletedTask;
    }
}
