using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Authenticator.Api.Core.Services;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Core.Accounts;

internal sealed class OtpAuthenticateHandler : IOtpAuthenticateHandler
{
    private readonly IAuthenticateService _authenticate;
    private readonly IOtpService _otp;
    private readonly IRepository _repository;

    public OtpAuthenticateHandler(
        IRepository repository,
        IAuthenticateService authenticate,
        IOtpService otp
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(authenticate);
        ArgumentNullException.ThrowIfNull(otp);
        this._authenticate = authenticate;
        this._otp = otp;
        this._repository = repository;
    }

    private async Task<IAuthenticateResult> HandleAsync(
        String emailAddress,
        String otp,
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

        return await this._authenticate.AuthenticateAsync(
            user,
            now,
            cancellationToken
        );
    }

    Task<IAuthenticateResult> IOtpAuthenticateHandler.HandleAsync(
        String emailAddress,
        String otp,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(emailAddress);
        ArgumentNullException.ThrowIfNull(otp);
        return this.HandleAsync(emailAddress, otp, cancellationToken);
    }
}
