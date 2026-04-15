using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;

/// <summary>
/// Represents a user refresh token repository.
/// </summary>
public interface IUserRefreshTokenRepository
{
    /// <summary>
    /// Asynchronously creates a user refresh token with the specified properties.
    /// </summary>
    /// <param name="userRefreshToken">The user refresh token to create.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous create operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="userRefreshToken" /></c> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task CreateAsync(
        UserRefreshTokenEntity userRefreshToken,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously deletes a user refresh token with the specified properties.
    /// </summary>
    /// <param name="userRefreshToken">The user refresh token to delete.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous delete operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="userRefreshToken" /></c> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task DeleteAsync(
        UserRefreshTokenEntity userRefreshToken,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously lists all user refresh tokens with the specified user ID.
    /// </summary>
    /// <param name="userId">A <see cref="Guid" /> containing the ID of the user to list user refresh tokens for.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous list operation. The value of <see cref="Task{TResult}.Result" /> contains the listed user refresh tokens.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="userId" /></c> is equal to <see cref="Guid.Empty" />.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<UserRefreshTokenEntity[]> ListForUserAsync(
        Guid userId,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously retrieves a user refresh token with the specified value.
    /// </summary>
    /// <param name="val">The value of the user refresh token to retrieve.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous retrieve operation. The value of <see cref="Task{TResult}.Result" /> contains the retrieved user refresh token, if found; otherwise, <c>null</c>.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="val" /></c> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<UserRefreshTokenEntity?> RetrieveAsync(
        String val,
        CancellationToken cancellationToken
    );
}
