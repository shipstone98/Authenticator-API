using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Authenticator.Api.Infrastructure.Entities;
using Shipstone.Authenticator.Api.Infrastructure.Mail;

namespace Shipstone.Authenticator.Api.Core.Services;

internal interface IOtpService
{
    Task GenerateAsync(
        UserEntity user,
        Func<IMailService, UserEntity, int, CancellationToken, Task> mailSend,
        CancellationToken cancellationToken
    );

    Task ValidateAsync(
        UserEntity user,
        String otp,
        DateTime now,
        CancellationToken cancellationToken
    );
}
