using System;

using Shipstone.Authenticator.Api.Core.Accounts;

namespace Shipstone.Authenticator.Api.Infrastructure.Authentication;

internal sealed record AuthenticateResult(
    String AccessToken,
    String RefreshToken,
    DateTime RefreshTokenExpires
)
    : IAuthenticateResult;
