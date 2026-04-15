using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities;

using Shipstone.Authenticator.Api.Infrastructure.Authentication;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;
using Shipstone.Authenticator.Api.Infrastructure.Mail;

namespace Shipstone.Authenticator.Api.Core.Services;

internal sealed class OtpService : IOtpService
{
    private readonly IAuthenticationService _authentication;
    private readonly IMailService _mail;
    private readonly IRepository _repository;

    public OtpService(
        IRepository repository,
        IAuthenticationService authentication,
        IMailService mail
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(authentication);
        ArgumentNullException.ThrowIfNull(mail);
        this._authentication = authentication;
        this._mail = mail;
        this._repository = repository;
    }

    async Task IOtpService.GenerateAsync(
        UserEntity user,
        Func<IMailService, UserEntity, int, CancellationToken, Task> mailSend,
        CancellationToken cancellationToken
    )
    {
        DateTime now = DateTime.UtcNow;

        await this._authentication.GenerateOtpAsync(
            user,
            now,
            cancellationToken
        );

        user.Updated = now;
        await this._repository.Users.UpdateAsync(user, cancellationToken);
        await this._repository.SaveAsync(cancellationToken);
        TimeSpan difference = user.OtpExpires!.Value.Subtract(now);

        await mailSend(
            this._mail,
            user,
            (int) difference.TotalMinutes,
            cancellationToken
        );
    }

    async Task IOtpService.ValidateAsync(
        UserEntity user,
        String otp,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        String? userOtp = user.Otp;
        Nullable<DateTime> userOtpExpires = user.OtpExpires;

        if (userOtp is null || !String.Equals(userOtp, otp))
        {
            throw new ForbiddenException("The provided OTP does not match the OTP for the user whose email address matches the provided email address.");
        }

        Nullable<DateTime> otpExpires = userOtpExpires;
        user.Otp = null;
        user.OtpExpires = null;
        user.Updated = now;
        await this._repository.Users.UpdateAsync(user, cancellationToken);
        await this._repository.SaveAsync(cancellationToken);

        if (
            !otpExpires.HasValue
            || DateTime.Compare(now, otpExpires.Value) > 0
        )
        {
            throw new ForbiddenException("The provided OTP has expired.");
        }
    }
}
