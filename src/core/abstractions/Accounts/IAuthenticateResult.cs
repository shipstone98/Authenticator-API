using System;

namespace Shipstone.Authenticator.Api.Core.Accounts;

/// <summary>
/// Represents the result of a successful authentication attempt.
/// </summary>
public interface IAuthenticateResult
{
    /// <summary>
    /// Gets the access token.
    /// </summary>
    /// <value>The access token.</value>
    String AccessToken { get; }

    /// <summary>
    /// Gets the refresh token.
    /// </summary>
    /// <value>The refresh token.</value>
    String RefreshToken { get; }

    /// <summary>
    /// Gets the date and time <see cref="IAuthenticateResult.RefreshToken" /> will expire.
    /// </summary>
    /// <value>The date and time <see cref="IAuthenticateResult.RefreshToken" /> will expire.</value>
    DateTime RefreshTokenExpires { get; }
}
