using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities;

namespace Shipstone.Authenticator.Api.Core.Users;

/// <summary>
/// Defines a method to handle updating the current user.
/// </summary>
public interface IUserUpdateHandler
{
    /// <summary>
    /// Asynchronously updates the current user with the specified forename and surname.
    /// </summary>
    /// <param name="forename">The forename for the user.</param>
    /// <param name="surname">The surname for the user.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous update operation. The value of <see cref="Task{TResult}.Result" /> contains the updated <see cref="IUser" />.</returns>
    /// <exception cref="ArgumentException"><c><paramref name="forename" /></c> is not a valid forename -or- <c><paramref name="surname" /></c> is not a valid surname.</exception>
    /// <exception cref="ArgumentNullException"><c><paramref name="forename" /></c> is <c>null</c> -or- <c><paramref name="surname" /></c> is <c>null</c>.</exception>
    /// <exception cref="NotFoundException">The current user could not be found.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    /// <exception cref="UserNotActiveException">The current user is not active.</exception>
    Task<IUser> HandleAsync(
        String forename,
        String surname,
        CancellationToken cancellationToken
    );
}
