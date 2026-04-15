using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Extensions.Identity;
using Shipstone.Utilities;

namespace Shipstone.Authenticator.Api.Core.Accounts;

/// <summary>
/// Defines a method to handle authentication.
/// </summary>
public interface IAuthenticateHandler
{
    /// <summary>
    /// Asynchronously authenticates an existing user with the specified email address and password.
    /// </summary>
    /// <param name="emailAddress">The email address of the user to authenticate.</param>
    /// <param name="password">The password of the user to authenticate.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous handle operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="emailAddress" /></c> is <c>null</c> -or- <c><paramref name="password" /></c> is <c>null</c>.</exception>
    /// <exception cref="ForbiddenException">The user whose email address matches the provided email address has not verified their email address.</exception>
    /// <exception cref="IncorrectPasswordException">The hashed representation of the provided password does not match the password hash of the user whose email address matches the provided email address.</exception>
    /// <exception cref="NotFoundException">A user whose email address matches the provided email address could not be found.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    /// <exception cref="UserNotActiveException">The user whose email address matches the provided email address is not active.</exception>
    Task HandleAsync(
        String emailAddress,
        String password,
        CancellationToken cancellationToken
    );
}
