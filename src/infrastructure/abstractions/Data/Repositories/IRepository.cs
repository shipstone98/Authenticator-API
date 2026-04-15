using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;

/// <summary>
/// Represents a repository.
/// </summary>
public interface IRepository
{
    /// <summary>
    /// Gets the user refresh token repository.
    /// </summary>
    /// <value>The user refresh token repository.</value>
    IUserRefreshTokenRepository UserRefreshTokens { get; }

    /// <summary>
    /// Gets the user repository.
    /// </summary>
    /// <value>The user repository.</value>
    IUserRepository Users { get; }

    /// <summary>
    /// Asynchronously saves all changes to the data source.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous save operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task SaveAsync(CancellationToken cancellationToken);
}
