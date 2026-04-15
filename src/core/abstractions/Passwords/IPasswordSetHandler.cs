using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Extensions.Identity;
using Shipstone.Utilities;

using Shipstone.Authenticator.Api.Core.Accounts;

namespace Shipstone.Authenticator.Api.Core.Passwords;

/// <summary>
/// Defines a method to handle password setting.
/// </summary>
public interface IPasswordSetHandler
{
    /// <summary>
    /// Asynchronously sets the password of the specified user.
    /// </summary>
    /// <param name="emailAddress">The email address of the user to set the password of.</param>
    /// <param name="otp">The one-time passcode (OTP) of the user to set the password of.</param>
    /// <param name="password">The new password for the user.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous set operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="emailAddress" /></c> is <c>null</c> -or- <c><paramref name="otp" /></c> is <c>null</c> -or- <c><paramref name="password" /></c> is <c>null</c>.</exception>
    /// <exception cref="ForbiddenException">The provided OTP does not match the OTP of the user whose email address matches the provided email address -or- the provided OTP has expired.</exception>
    /// <exception cref="NotFoundException">A user whose email address matches the provided email address could not be found.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    /// <exception cref="PasswordNotValidException"><c><paramref name="password" /></c> is not a valid password.</exception>
    /// <exception cref="UserNotActiveException">The user whose email address matches the provided email address is not active.</exception>
    Task HandleAsync(
        String emailAddress,
        String otp,
        String password,
        CancellationToken cancellationToken
    );
}
