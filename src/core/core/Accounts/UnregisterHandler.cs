using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;
using Shipstone.Authenticator.Api.Infrastructure.Mail;

namespace Shipstone.Authenticator.Api.Core.Accounts;

internal sealed class UnregisterHandler : IUnregisterHandler
{
    private readonly IClaimsService _claims;
    private readonly IMailService _mail;
    private readonly IRepository _repository;

    public UnregisterHandler(
        IRepository repository,
        IClaimsService claims,
        IMailService mail
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(mail);
        this._claims = claims;
        this._mail = mail;
        this._repository = repository;
    }

    private async Task DeleteUserRefreshTokensAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<UserRefreshTokenEntity> userRefreshTokens =
            await this._repository.UserRefreshTokens.ListForUserAsync(
                userId,
                cancellationToken
            );

        foreach (UserRefreshTokenEntity userRefreshToken in userRefreshTokens)
        {
            await this._repository.UserRefreshTokens.DeleteAsync(
                userRefreshToken,
                cancellationToken
            );
        }
    }

    async Task IUnregisterHandler.HandleAsync(CancellationToken cancellationToken)
    {
        UserEntity user =
            await this._claims.RetrieveActiveUserAsync(
                this._repository,
                cancellationToken
            );

        user.Born = DateOnly.MinValue;
        user.EmailAddress = String.Empty;
        user.EmailAddressNormalized = null;
        user.Forename = String.Empty;
        user.IsActive = false;
        user.Otp = null;
        user.OtpExpires = null;
        user.PasswordHash = null;
        user.Surname = String.Empty;
        user.Updated = DateTime.UtcNow;
        Guid userId = user.Id;
        await this.DeleteUserRefreshTokensAsync(userId, cancellationToken);
        await this._repository.Users.UpdateAsync(user, cancellationToken);
        await this._repository.SaveAsync(cancellationToken);

        await this._mail.SendUnregistrationAsync(
            this._claims.EmailAddress,
            cancellationToken
        );
    }
}
