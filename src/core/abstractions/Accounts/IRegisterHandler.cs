using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities;

namespace Shipstone.Authenticator.Api.Core.Accounts;

/// <summary>
/// Defines a method to handle registration.
/// </summary>
public interface IRegisterHandler
{
    /// <summary>
    /// Asynchronously registers a new user with the specified properties.
    /// </summary>
    /// <param name="emailAddress">The email address for the new user.</param>
    /// <param name="forename">The forename for the new user.</param>
    /// <param name="surname">The surname for the new user.</param>
    /// <param name="born">The date the new user was born.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous register operation. The value of <see cref="Task{TResult}.Result" /> contains the new <see cref="IUser" />.</returns>
    /// <exception cref="ArgumentException"><c><paramref name="emailAddress" /></c> is not a valid email address -or- <c><paramref name="forename" /></c> is not a valid forename -or- <c><paramref name="surname" /></c> is not a valid surname -or- <c><paramref name="born" /></c> is not a valid date of birth.</exception>
    /// <exception cref="ArgumentNullException"><c><paramref name="emailAddress" /></c> is <c>null</c> -or- -or- <c><paramref name="forename" /></c> is <c>null</c> -or- <c><paramref name="surname" /></c> is <c>null</c>.</exception>
    /// <exception cref="ConflictException">A user whose email address and/or name matches the provided email address and/or user name already exists.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<IUser> HandleAsync(
        String emailAddress,
        String forename,
        String surname,
        DateOnly born,
        CancellationToken cancellationToken
    );
}
