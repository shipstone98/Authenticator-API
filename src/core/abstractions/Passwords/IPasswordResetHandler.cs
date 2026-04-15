using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shipstone.Authenticator.Api.Core.Passwords;

/// <summary>
/// Defines a method to handle password resetting.
/// </summary>
public interface IPasswordResetHandler
{
    /// <summary>
    /// Asynchronously resets the password of the specified user.
    /// </summary>
    /// <param name="emailAddress">The email address of the user to reset the password of.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous reset operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="emailAddress" /></c> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task HandleAsync(String emailAddress, CancellationToken cancellationToken);
}
