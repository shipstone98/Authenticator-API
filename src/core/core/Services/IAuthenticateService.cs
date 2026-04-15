using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Authenticator.Api.Core.Accounts;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Core.Services;

internal interface IAuthenticateService
{
    Task<IAuthenticateResult> AuthenticateAsync(
        UserEntity user,
        DateTime now,
        CancellationToken cancellationToken
    );
}
