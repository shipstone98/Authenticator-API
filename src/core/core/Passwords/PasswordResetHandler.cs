using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Authenticator.Api.Core.Services;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Core.Passwords;

internal sealed class PasswordResetHandler : IPasswordResetHandler
{
    private readonly IOtpService _otp;
    private readonly IRepository _repository;

    public PasswordResetHandler(IRepository repository, IOtpService otp)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(otp);
        this._otp = otp;
        this._repository = repository;
    }

    private async Task HandleAsync(
        String emailAddress,
        CancellationToken cancellationToken
    )
    {
        UserEntity? user =
            await this._repository.Users.RetrieveAsync(
                emailAddress,
                cancellationToken
            );

        if (user is null || !user.IsActive)
        {
            return;
        }

        await this._otp.GenerateAsync(
            user,
            (m, u, em, ct) => m.SendPasswordResetAsync(u, em, ct),
            cancellationToken
        );
    }

    Task IPasswordResetHandler.HandleAsync(
        String emailAddress,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(emailAddress);
        return this.HandleAsync(emailAddress, cancellationToken);
    }
}
