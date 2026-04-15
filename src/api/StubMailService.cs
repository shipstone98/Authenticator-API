using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Authenticator.Api.Infrastructure.Entities;
using Shipstone.Authenticator.Api.Infrastructure.Mail;

namespace Shipstone.Authenticator.Api.WebApi;

internal sealed class StubMailService : IMailService
{
    Task IMailService.SendOtpAsync(
        UserEntity user,
        int expiryMinutes,
        CancellationToken cancellationToken
    ) =>
        Task.CompletedTask;

    Task IMailService.SendPasswordResetAsync(
        UserEntity user,
        int expiryMinutes,
        CancellationToken cancellationToken
    ) =>
        throw new NotImplementedException();

    Task IMailService.SendRegistrationAsync(
        UserEntity user,
        int expiryMinutes,
        CancellationToken cancellationToken
    ) =>
        Task.CompletedTask;

    Task IMailService.SendUnregistrationAsync(
        String emailAddress,
        CancellationToken cancellationToken
    ) =>
        Task.CompletedTask;
}
