using System;

using Shipstone.Authenticator.Api.Core;

namespace Shipstone.Authenticator.Api.Infrastructure.Entities;

/// <summary>
/// Represents a user.
/// </summary>
public class UserEntity : Entity<Guid>
{
    private String _emailAddress;
    private String _forename;
    private String? _otp;
    private String _surname;

    /// <summary>
    /// Gets or sets the date the user was born.
    /// </summary>
    /// <value>The date the user was born.</value>
    public DateOnly Born { get; set; }

    /// <summary>
    /// Gets or sets the date and time the user last consented to policy.
    /// </summary>
    /// <value>The date and time the user last consented to policy.</value>
    public DateTime Consented { get; set; }

    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    /// <value>The email address of the user, if the user is active; otherwise, <c>null</c>.</value>
    /// <exception cref="ArgumentNullException">The property is set and the value is <c>null</c>.</exception>
    public String EmailAddress
    {
        get => this._emailAddress;

        set
        {
            ArgumentNullException.ThrowIfNull(value);
            this._emailAddress = value;
        }
    }

    /// <summary>
    /// Gets or sets the normalized email address of the user.
    /// </summary>
    /// <value>The normalized email address of the user, if the user is active; otherwise, <c>null</c>.</value>
    public String? EmailAddressNormalized { get; set; }

    /// <summary>
    /// Gets or sets the forename of the user.
    /// </summary>
    /// <value>The forename of the user, if the user is active; otherwise, <c>null</c>.</value>
    /// <exception cref="ArgumentNullException">The property is set and the value is <c>null</c>.</exception>
    public String Forename
    {
        get => this._forename;

        set
        {
            ArgumentNullException.ThrowIfNull(value);
            this._forename = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the user is active.
    /// </summary>
    /// <value><c>true</c> if the user is active; otherwise, <c>false</c>.</value>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the one-time passcode (OTP) of the user.
    /// </summary>
    /// <value>The one-time passcode (OTP) of the user, if the user is active; otherwise, <c>null</c>.</value>
    /// <exception cref="ArgumentNullException">The property is set and the length of the value is greater than <see cref="Constants.UserOtpMaxLength "/>.</exception>
    public String? Otp
    {
        get => this._otp;

        set
        {
            if (value is null)
            {
                this._otp = null;
            }

            else if (value.Length > Constants.UserOtpMaxLength)
            {
                throw new ArgumentException(
                    $"The length of {nameof (value)} is greater than Constants.UserOtpMaxLength.",
                    nameof (value)
                );
            }

            else
            {
                this._otp = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the date and time the OTP (one-time passcode) of the user will expire.
    /// </summary>
    /// <value>The date and time the OTP (one-time passcode) of the user will expire, or <c>null</c> if <see cref="UserEntity.Otp" /> is <c>null</c>.</value>
    public Nullable<DateTime> OtpExpires { get; set; }

    /// <summary>
    /// Gets or sets the password hash of the user.
    /// </summary>
    /// <value>The password hash of the user, or <c>null</c>.</value>
    public String? PasswordHash { get; set; }

    /// <summary>
    /// Gets or sets the surname of the user.
    /// </summary>
    /// <value>The surname of the user, if the user is active; otherwise, <c>null</c>.</value>
    /// <exception cref="ArgumentNullException">The property is set and the value is <c>null</c>.</exception>
    public String Surname
    {
        get => this._surname;

        set
        {
            ArgumentNullException.ThrowIfNull(value);
            this._surname = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserEntity" /> class.
    /// </summary>
    public UserEntity()
    {
        this._emailAddress = String.Empty;
        this._forename = String.Empty;
        this._surname = String.Empty;
    }
}
