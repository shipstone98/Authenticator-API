using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Utilities;

using Shipstone.Authenticator.Api.Core.Services;
using Shipstone.Authenticator.Api.Infrastructure.Authentication;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Core.Accounts;

internal sealed class RefreshAuthenticateHandler : IRefreshAuthenticateHandler
{
    private readonly IAuthenticateService _authenticate;
    private readonly IAuthenticationService _authentication;
    private readonly IRepository _repository;

    public RefreshAuthenticateHandler(
        IRepository repository,
        IAuthenticateService authenticate,
        IAuthenticationService authentication
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(authenticate);
        ArgumentNullException.ThrowIfNull(authentication);
        this._authenticate = authenticate;
        this._authentication = authentication;
        this._repository = repository;
    }

    private async Task<IAuthenticateResult> HandleAsync(
        String refreshToken,
        CancellationToken cancellationToken
    )
    {
        Guid id;

        try
        {
            id = this._authentication.GetId(refreshToken);
        }

        catch (ArgumentException ex)
        {
            throw new ForbiddenException(
                "The provided refresh token is not valid.",
                ex
            );
        }

        UserRefreshTokenEntity? userRefreshToken =
            await this._repository.UserRefreshTokens.RetrieveAsync(
                refreshToken,
                cancellationToken
            );

        if (userRefreshToken is null)
        {
            throw new NotFoundException("A user refresh token whose value matches the provided refresh token could not be found.");
        }

        await this._repository.UserRefreshTokens.DeleteAsync(
            userRefreshToken,
            cancellationToken
        );

        await this._repository.SaveAsync(cancellationToken);
        DateTime now = DateTime.UtcNow;

        if (DateTime.Compare(now, userRefreshToken.Expires) > 0)
        {
            throw new ForbiddenException("The user refresh token whose value matches the provided refresh token has expired.");
        }

        UserEntity? user =
            await this._repository.Users.RetrieveAsync(id, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("A user whose ID matches the user ID of the user refresh token whose value matches the provided refresh token does not match the ID claimed by the provided refresh token could not be found.");
        }

        if (!user.IsActive)
        {
            throw new UserNotActiveException("The user whose ID matches the user ID of the user refresh token whose value matches the provided refresh token does not match the ID claimed by the provided refresh token is not active.");
        }

        return await this._authenticate.AuthenticateAsync(
            user,
            now,
            cancellationToken
        );
    }

    Task<IAuthenticateResult> IRefreshAuthenticateHandler.HandleAsync(
        String refreshToken,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(refreshToken);
        return this.HandleAsync(refreshToken, cancellationToken);
    }
}
