using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Infrastructure.Mail;

/// <summary>
/// Defines methods to send electronic mail.
/// </summary>
public interface IMailService
{
    /// <summary>
    /// Asynchronously sends one-time passcode (OTP) mail to the specified user.
    /// </summary>
    /// <param name="user">The user to send mail to.</param>
    /// <param name="expiryMinutes">The expiry time for the one-time passcode, expressed as minutes.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous send operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="user" /></c> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="expiryMinutes" /></c> is less than or equal to 0 (zero).</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task SendOtpAsync(
        UserEntity user,
        int expiryMinutes,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously sends password reset mail to the specified user.
    /// </summary>
    /// <param name="user">The user to send mail to.</param>
    /// <param name="expiryMinutes">The expiry time for the one-time passcode, expressed as minutes.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous send operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="user" /></c> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="expiryMinutes" /></c> is less than or equal to 0 (zero).</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task SendPasswordResetAsync(
        UserEntity user,
        int expiryMinutes,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously sends registration mail to the specified user.
    /// </summary>
    /// <param name="user">The user to send mail to.</param>
    /// <param name="expiryMinutes">The expiry time for the one-time passcode, expressed as minutes.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous send operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="user" /></c> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="expiryMinutes" /></c> is less than or equal to 0 (zero).</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task SendRegistrationAsync(
        UserEntity user,
        int expiryMinutes,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously sends unregistration mail to the specified email address.
    /// </summary>
    /// <param name="emailAddress">The email address to send mail to.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous send operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="emailAddress" /></c> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task SendUnregistrationAsync(
        String emailAddress,
        CancellationToken cancellationToken
    );
}
