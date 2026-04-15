using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Authenticator.Api.Core.Accounts;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Infrastructure.Authentication;

/// <summary>
/// Defines methods to handle authentication.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Asynchronously authenticates the specified user.
    /// </summary>
    /// <param name="user">The user to authenticate.</param>
    /// <param name="now">The date and time the user was authenticated.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous authenticate operation. The value of <see cref="Task{TResult}.Result" /> contains the <see cref="IAuthenticateResult" />.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="user" /></c> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<IAuthenticateResult> AuthenticateAsync(
        UserEntity user,
        DateTime now,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously generates a one-time passcode (OTP) for the specified user.
    /// </summary>
    /// <param name="user">The user to generate a one-time passcode for.</param>
    /// <param name="now">The current date and time.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous generate operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="user" /></c> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task GenerateOtpAsync(
        UserEntity user,
        DateTime now,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Returns the ID claimed by the specified token.
    /// </summary>
    /// <param name="token">The token to retrieve the claimed ID of.</param>
    /// <returns>The ID claimed by <c><paramref name="token" /></c>.</returns>
    /// <exception cref="ArgumentException"><c><paramref name="token" /></c> is not a valid token.</exception>
    /// <exception cref="ArgumentNullException"><c><paramref name="token" /></c> is <c>null</c>.</exception>
    Guid GetId(String token);
}
