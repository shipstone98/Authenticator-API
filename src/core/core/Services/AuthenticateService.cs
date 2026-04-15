using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Authenticator.Api.Core.Accounts;
using Shipstone.Authenticator.Api.Infrastructure.Authentication;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Core.Services;

internal sealed class AuthenticateService : IAuthenticateService
{
    private readonly IAuthenticationService _authentication;
    private readonly IRepository _repository;

    public AuthenticateService(
        IRepository repository,
        IAuthenticationService authentication
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(authentication);
        this._authentication = authentication;
        this._repository = repository;
    }

    async Task<IAuthenticateResult> IAuthenticateService.AuthenticateAsync(
        String audience,
        UserEntity user,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        Guid userId = user.Id;

        IAuthenticateResult result =
            await this._authentication.AuthenticateAsync(
                audience,
                user,
                now,
                cancellationToken
            );

        await this._repository.UserRefreshTokens.CreateAsync(
            new UserRefreshTokenEntity
            {
                Created = now,
                Expires = result.RefreshTokenExpires,
                Updated = now,
                UserId = userId,
                Value = result.RefreshToken
            },
            cancellationToken
        );

        await this._repository.SaveAsync(cancellationToken);
        return result;
    }
}
