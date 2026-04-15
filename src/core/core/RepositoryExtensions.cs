using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities;

using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Core;

internal static class RepositoryExtensions
{
    internal static async Task<UserEntity> RetrieveActiveUserAsync(
        this IRepository repository,
        String emailAddress,
        CancellationToken cancellationToken
    )
    {
        UserEntity? user =
            await repository.Users.RetrieveAsync(
                emailAddress,
                cancellationToken
            );

        if (user is null)
        {
            throw new NotFoundException("A user whose email address matches the provided email address could not be found.");
        }

        if (!user.IsActive)
        {
            throw new UserNotActiveException("The user whose email address matches the provided email address is not active.");
        }

        return user;
    }
}
