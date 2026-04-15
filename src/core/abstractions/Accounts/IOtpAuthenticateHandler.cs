using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities;

namespace Shipstone.Authenticator.Api.Core.Accounts;

/// <summary>
/// Defines a method to handle one-time passcode (OTP) authentication.
/// </summary>
public interface IOtpAuthenticateHandler
{
    /// <summary>
    /// Asynchronously authenticates an existing user with the specified email address and one-time passcode (OTP).
    /// </summary>
    /// <param name="emailAddress">The email address of the user to authenticate.</param>
    /// <param name="otp">The one-time passcode (OTP) of the user to authenticate.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous handle operation. The value of <see cref="Task{TResult}.Result" /> contains the <see cref="IAuthenticateResult" />.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="emailAddress" /></c> is <c>null</c> -or- <c><paramref name="otp" /></c> is <c>null</c>.</exception>
    /// <exception cref="ForbiddenException">The provided OTP does not match the OTP of the user whose email address matches the provided email address -or- the provided OTP has expired.</exception>
    /// <exception cref="NotFoundException">A user whose email address matches the provided email address could not be found.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    /// <exception cref="UserNotActiveException">The user whose email address matches the provided email address is not active.</exception>
    Task<IAuthenticateResult> HandleAsync(
        String emailAddress,
        String otp,
        CancellationToken cancellationToken
    );
}
