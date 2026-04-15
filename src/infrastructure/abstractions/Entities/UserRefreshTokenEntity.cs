using System;

namespace Shipstone.Authenticator.Api.Infrastructure.Entities;

/// <summary>
/// Represents a user refresh token.
/// </summary>
public class UserRefreshTokenEntity : Entity<long>
{
    private String _value;

    /// <summary>
    /// Gets or initializes the date and time the user refresh token will expire.
    /// </summary>
    /// <value>The date and time the user refresh token will expire.</value>
    public DateTime Expires { get; init; }

    /// <summary>
    /// Gets the ID of the associated user.
    /// </summary>
    /// <value>A <see cref="Guid" /> containing the ID of the associated user.
    public Guid UserId { get; init; }

    /// <summary>
    /// Gets or initializes the value of the user refresh token.
    /// </summary>
    /// <value>The value of the user refresh token.</value>
    /// <exception cref="ArgumentNullException">The property is initialized and the value is <c>null</c>.</exception>
    public String Value
    {
        get => this._value;

        init
        {
            ArgumentNullException.ThrowIfNull(value);
            this._value = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRefreshTokenEntity" /> class.
    /// </summary>
    public UserRefreshTokenEntity() => this._value = String.Empty;
}
