using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Extensions.Identity;

using Shipstone.Authenticator.Api.Core.Services;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Core.Passwords;

internal sealed class PasswordSetHandler : IPasswordSetHandler
{
    private readonly IOtpService _otp;
    private readonly IPasswordService _password;
    private readonly IRepository _repository;

    public PasswordSetHandler(
        IRepository repository,
        IOtpService otp,
        IPasswordService password
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(otp);
        ArgumentNullException.ThrowIfNull(password);
        this._otp = otp;
        this._password = password;
        this._repository = repository;
    }

    private async Task HandleAsync(
        String emailAddress,
        String otp,
        String password,
        CancellationToken cancellationToken
    )
    {
        UserEntity user =
            await this._repository.RetrieveActiveUserAsync(
                emailAddress,
                cancellationToken
            );

        DateTime now = DateTime.UtcNow;
        await this._otp.ValidateAsync(user, otp, now, cancellationToken);
        user.PasswordHash = this._password.Hash(password);
        user.Updated = now;
        await this._repository.Users.UpdateAsync(user, cancellationToken);
        await this._repository.SaveAsync(cancellationToken);
    }

    Task IPasswordSetHandler.HandleAsync(
        String emailAddress,
        String otp,
        String password,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(emailAddress);
        ArgumentNullException.ThrowIfNull(otp);
        this._password.Validate(password);

        return this.HandleAsync(
            emailAddress,
            otp,
            password,
            cancellationToken
        );
    }
}
