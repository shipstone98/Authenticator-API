using System;

using Shipstone.Utilities;

namespace Shipstone.Authenticator.Api.Core;

/// <summary>
/// Defines properties to retrieve values claimed by the current user.
/// </summary>
public interface IClaimsService
{
    /// <summary>
    /// Gets the email address of the current user.
    /// </summary>
    /// <value>The email address of the current user.</value>
    /// <exception cref="UnauthorizedException">The current user is not authenticated.</exception>
    String EmailAddress { get; }

    /// <summary>
    /// Gets the ID of the current user.
    /// </summary>
    /// <value>A <see cref="Guid" /> containing the ID of the current user.</value>
    /// <exception cref="UnauthorizedException">The current user is not authenticated.</exception>
    Guid Id { get; }
}
