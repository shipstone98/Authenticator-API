using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.EntityFrameworkCore;

using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Infrastructure.Data;

/// <summary>
/// Represents a data source.
/// </summary>
public interface IDataSource
{
    /// <summary>
    /// Gets the user refresh token data set.
    /// </summary>
    /// <value>An <see cref="IDataSet{TEntity}" /> containing the user refresh tokens.</value>
    IDataSet<UserRefreshTokenEntity> UserRefreshTokens { get; }

    /// <summary>
    /// Gets the user data set.
    /// </summary>
    /// <value>An <see cref="IDataSet{TEntity}" /> containing the users.</value>
    IDataSet<UserEntity> Users { get; }

    /// <summary>
    /// Asynchronously saves all changes to the data source.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous save operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task SaveAsync(CancellationToken cancellationToken);
}
