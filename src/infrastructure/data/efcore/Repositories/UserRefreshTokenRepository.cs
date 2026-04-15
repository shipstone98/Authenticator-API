using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Shipstone.EntityFrameworkCore;

using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;

internal sealed class UserRefreshTokenRepository : IUserRefreshTokenRepository
{
    private readonly IDataSource _dataSource;

    public UserRefreshTokenRepository(IDataSource dataSource)
    {
        ArgumentNullException.ThrowIfNull(dataSource);
        this._dataSource = dataSource;
    }

    Task IUserRefreshTokenRepository.CreateAsync(
        UserRefreshTokenEntity userRefreshToken,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(userRefreshToken);

        return this._dataSource.UserRefreshTokens.SetStateAsync(
            userRefreshToken,
            DataEntityState.Created,
            cancellationToken
        );
    }

    Task IUserRefreshTokenRepository.DeleteAsync(
        UserRefreshTokenEntity userRefreshToken,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(userRefreshToken);

        return this._dataSource.UserRefreshTokens.SetStateAsync(
            userRefreshToken,
            DataEntityState.Deleted,
            cancellationToken
        );
    }

#warning Not tested
    Task<UserRefreshTokenEntity[]> IUserRefreshTokenRepository.ListForUserAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        if (Guid.Equals(userId, Guid.Empty))
        {
            throw new ArgumentException(
                $"{nameof (userId)} is equal to Guid.Empty.",
                nameof (userId)
            );
        }

        return this._dataSource.UserRefreshTokens
            .Where(urt => Guid.Equals(userId, urt.UserId))
            .ToArrayAsync(cancellationToken);
    }

    Task<UserRefreshTokenEntity?> IUserRefreshTokenRepository.RetrieveAsync(
        String val,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(val);

        return this._dataSource.UserRefreshTokens.FirstOrDefaultAsync(
            urt => String.Equals(val, urt.Value),
            cancellationToken
        );
    }
}
