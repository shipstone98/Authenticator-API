using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Shipstone.EntityFrameworkCore;
using Shipstone.Extensions.Security;

using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly IDataSource _dataSource;
    private readonly INormalizationService _normalization;

    public UserRepository(
        IDataSource dataSource,
        INormalizationService normalization
    )
    {
        ArgumentNullException.ThrowIfNull(dataSource);
        ArgumentNullException.ThrowIfNull(normalization);
        this._dataSource = dataSource;
        this._normalization = normalization;
    }

    private async Task<UserEntity?> RetrieveAsync(
        String emailAddress,
        CancellationToken cancellationToken
    )
    {
        String emailAddressNormalized =
            this._normalization.Normalize(emailAddress);

        IEnumerable<UserEntity> users =
            await this._dataSource.Users
                .Where(u =>
                    emailAddressNormalized.Equals(u.EmailAddressNormalized))
                .ToArrayAsync(cancellationToken);

        return users.FirstOrDefault(u => emailAddress.Equals(u.EmailAddress));
    }

    Task IUserRepository.CreateAsync(
        UserEntity user,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(user);

        return this._dataSource.Users.SetStateAsync(
            user,
            DataEntityState.Created,
            cancellationToken
        );
    }

    Task<UserEntity?> IUserRepository.RetrieveAsync(
        String emailAddress,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(emailAddress);
        return this.RetrieveAsync(emailAddress, cancellationToken);
    }

    Task<UserEntity?> IUserRepository.RetrieveAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        if (Guid.Equals(id, Guid.Empty))
        {
            throw new ArgumentException(
                $"{nameof (id)} is equal to Guid.Empty.",
                nameof (id)
            );
        }

        return this._dataSource.Users.FirstOrDefaultAsync(
            u => Guid.Equals(id, u.Id),
            cancellationToken
        );
    }

    Task IUserRepository.UpdateAsync(
        UserEntity user,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(user);

        return this._dataSource.Users.SetStateAsync(
            user,
            DataEntityState.Updated,
            cancellationToken
        );
    }
}
