using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities;

namespace Shipstone.Authenticator.Api.Core.Accounts;

/// <summary>
/// Defines a method to handle refresh authentication.
/// </summary>
public interface IRefreshAuthenticateHandler
{
    /// <summary>
    /// Asynchronously authenticates an existing user with the specified refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token of the user to authenticate.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous handle operation. The value of <see cref="Task{TResult}.Result" /> contains the <see cref="IAuthenticateResult" />.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="refreshToken" /></c> is <c>null</c>.</exception>
    /// <exception cref="ForbiddenException">The audience claimed by the provided refresh token is not authorized -or- the provided refresh token is not valid -or- the user refresh token whose value matches the provided refresh token has expired.</exception>
    /// <exception cref="NotFoundException">A user refresh token whose value matches the provided refresh token could not be found -or- a user whose ID matches the user ID of the user refresh token whose value matches the provided refresh token could not be found.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    /// <exception cref="UserNotActiveException">The user whose ID matches the user ID of the user refresh token whose value matches the provided refresh token is not active.</exception>
    Task<IAuthenticateResult> HandleAsync(
        String refreshToken,
        CancellationToken cancellationToken
    );
}
