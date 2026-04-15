using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities;

using Shipstone.Authenticator.Api.Core.Accounts;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Core;

internal static class ClaimsServiceExtensions
{
    internal static async Task<UserEntity> RetrieveActiveUserAsync(
        this IClaimsService claims,
        IRepository repository,
        CancellationToken cancellationToken
    )
    {
        UserEntity? user =
            await repository.Users.RetrieveAsync(
                claims.Id,
                cancellationToken
            );

        if (user is null)
        {
            throw new NotFoundException("The current user could not be found.");
        }

        if (!user.IsActive)
        {
            throw new UserNotActiveException("The current user is not active.");
        }

        return user;
    }
}
