using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Extensions.Identity;

using Shipstone.Authenticator.Api.Core.Accounts;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Core.Passwords;

internal sealed class PasswordUpdateHandler : IPasswordUpdateHandler
{
    private readonly IClaimsService _claims;
    private readonly IPasswordService _password;
    private readonly IRepository _repository;

    public PasswordUpdateHandler(
        IRepository repository,
        IClaimsService claims,
        IPasswordService password
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(password);
        this._claims = claims;
        this._password = password;
        this._repository = repository;
    }

    private async Task HandleAsync(
        String passwordCurrent,
        String passwordNew,
        CancellationToken cancellationToken
    )
    {
        UserEntity user =
            await this._claims.RetrieveActiveUserAsync(
                this._repository,
                cancellationToken
            );

        this._password.Verify(user, passwordCurrent);
        user.PasswordHash = this._password.Hash(passwordNew);
        user.Updated = DateTime.UtcNow;
        await this._repository.Users.UpdateAsync(user, cancellationToken);
        await this._repository.SaveAsync(cancellationToken);
    }

    Task IPasswordUpdateHandler.HandleAsync(
        String passwordCurrent,
        String passwordNew,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(passwordCurrent);
        this._password.Validate(passwordNew);

        return this.HandleAsync(
            passwordCurrent,
            passwordNew,
            cancellationToken
        );
    }
}
