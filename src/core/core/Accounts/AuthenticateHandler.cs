using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Extensions.Identity;

using Shipstone.Authenticator.Api.Core.Services;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Core.Accounts;

internal sealed class AuthenticateHandler : IAuthenticateHandler
{
    private readonly IOtpService _otp;
    private readonly IPasswordService _password;
    private readonly IRepository _repository;

    public AuthenticateHandler(
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
        String password,
        CancellationToken cancellationToken
    )
    {
        UserEntity user =
            await this._repository.RetrieveActiveUserAsync(
                emailAddress,
                cancellationToken
            );

        if (!this._password.Verify(user, password))
        {
            user.PasswordHash = this._password.Hash(password);
        }

        await this._otp.GenerateAsync(
            user,
            (m, u, em, ct) => m.SendOtpAsync(u, em, ct),
            cancellationToken
        );
    }

    Task IAuthenticateHandler.HandleAsync(
        String emailAddress,
        String password,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(emailAddress);
        ArgumentNullException.ThrowIfNull(password);
        return this.HandleAsync(emailAddress, password, cancellationToken);
    }
}
