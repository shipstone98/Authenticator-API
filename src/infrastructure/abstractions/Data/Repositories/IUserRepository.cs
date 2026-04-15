using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;

/// <summary>
/// Represents a user repository.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Asynchronously creates a user with the specified properties.
    /// </summary>
    /// <param name="user">The user to create.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous create operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="user" /></c> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task CreateAsync(UserEntity user, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously retrieves a user with the specified email address.
    /// </summary>
    /// <param name="emailAddress">The email address of the user to retrieve.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous retrieve operation. The value of <see cref="Task{TResult}.Result" /> contains the retrieved user, if found; otherwise, <c>null</c>.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="emailAddress" /></c> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<UserEntity?> RetrieveAsync(
        String emailAddress,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously retrieves a user with the specified ID.
    /// </summary>
    /// <param name="id">A <see cref="Guid" /> containing the ID of the user to retrieve.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous retrieve operation. The value of <see cref="Task{TResult}.Result" /> contains the retrieved user, if found; otherwise, <c>null</c>.</returns>
    /// <exception cref="ArgumentException"><c><paramref name="id" /></c> is equal to <see cref="Guid.Empty" />.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<UserEntity?> RetrieveAsync(
        Guid id,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously updates a user with the specified properties.
    /// </summary>
    /// <param name="user">The user to update.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous update operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="user" /></c> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task UpdateAsync(UserEntity user, CancellationToken cancellationToken);
}
