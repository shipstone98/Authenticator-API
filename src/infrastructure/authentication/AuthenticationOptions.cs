using System;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Shipstone.Authenticator.Api.Infrastructure.Authentication;

/// <summary>
/// Specifies options for authentication requirements.
/// </summary>
public class AuthenticationOptions : IOptions<AuthenticationOptions>
{
    /// <summary>
    /// Represents the minimum length of a signing key, in bytes. This field is constant.
    /// </summary>
    public const int SigningKeyMinLength = 32;

    internal TimeSpan _accessTokenExpiry;
    internal SigningCredentials _accessTokenSigningKey;
    private String _accessTokenSigningKeyString;
    internal String _audience;
    internal String _issuer;
    internal TimeSpan _otpExpiry;
    internal TimeSpan _refreshTokenExpiry;
    internal SigningCredentials _refreshTokenSigningKey;
    private String _refreshTokenSigningKeyString;

    public int AccessTokenExpiryMinutes
    {
        get => (int) this._accessTokenExpiry.TotalMinutes;

        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0);
            this._accessTokenExpiry = TimeSpan.FromMinutes(value);
        }
    }

    public String AccessTokenSigningKey
    {
        get => this._accessTokenSigningKeyString;

        set
        {
            ArgumentNullException.ThrowIfNull(value);

            try
            {
                byte[] bytes = Convert.FromBase64String(value);

                if (bytes.Length < AuthenticationOptions.SigningKeyMinLength)
                {
                    throw new ArgumentException(
                        $"The length of {nameof (value)} is less than {nameof (AuthenticationOptions.SigningKeyMinLength)}.",
                        nameof (value)
                    );
                }

                SecurityKey key = new SymmetricSecurityKey(bytes);

                SigningCredentials signingKey =
                    new(key, SecurityAlgorithms.HmacSha256);

                this._accessTokenSigningKey = signingKey;
                this._accessTokenSigningKeyString = value;
            }

            catch (Exception ex)
            {
                throw new FormatException(
                    $"{nameof (value)} is not in the correct format.",
                    ex
                );
            }
        }
    }

    /// <summary>
    /// Gets or sets the audience for generated authentication tokens.
    /// </summary>
    /// <value>The entity that is the intended audience for generated authentication tokens.</value>
    /// <exception cref="ArgumentNullException">The property is set and the value is <c>null</c>.</exception>
    public String Audience
    {
        get => this._audience;

        set
        {
            ArgumentNullException.ThrowIfNull(value);
            this._audience = value;
        }
    }

    /// <summary>
    /// Gets or sets the issuer of generated authentication tokens.
    /// </summary>
    /// <value>The authority that is the issuer of generated authentication tokens.</value>
    /// <exception cref="ArgumentNullException">The property is set and the value is <c>null</c>.</exception>
    public String Issuer
    {
        get => this._issuer;

        set
        {
            ArgumentNullException.ThrowIfNull(value);
            this._issuer = value;
        }
    }

    /// <summary>
    /// Gets or sets the expiry for one-time passcodes, expressed as minutes.
    /// </summary>
    /// <value>The expiry for one-time passcodes.</value>
    /// <exception cref="ArgumentOutOfRangeException">The property is set and the value is less than or equal to 0 (zero).</exception>
    public int OtpExpiryMinutes
    {
        get => (int) this._otpExpiry.TotalMinutes;

        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0);
            this._otpExpiry = TimeSpan.FromMinutes(value);
        }
    }

    public int RefreshTokenExpiryMinutes
    {
        get => (int) this._refreshTokenExpiry.TotalMinutes;

        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0);
            this._refreshTokenExpiry = TimeSpan.FromMinutes(value);
        }
    }

    public String RefreshTokenSigningKey
    {
        get => this._refreshTokenSigningKeyString;

        set
        {
            ArgumentNullException.ThrowIfNull(value);

            try
            {
                byte[] bytes = Convert.FromBase64String(value);

                if (bytes.Length < AuthenticationOptions.SigningKeyMinLength)
                {
                    throw new ArgumentException(
                        $"The length of {nameof (value)} is less than {nameof (AuthenticationOptions.SigningKeyMinLength)}.",
                        nameof (value)
                    );
                }

                SecurityKey key = new SymmetricSecurityKey(bytes);

                SigningCredentials signingKey =
                    new(key, SecurityAlgorithms.HmacSha256);

                this._refreshTokenSigningKey = signingKey;
                this._refreshTokenSigningKeyString = value;
            }

            catch (Exception ex)
            {
                throw new FormatException(
                    $"{nameof (value)} is not in the correct format.",
                    ex
                );
            }
        }
    }

    AuthenticationOptions IOptions<AuthenticationOptions>.Value => this;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationOptions" /> class.
    /// </summary>
    public AuthenticationOptions()
    {
        byte[] bytes = new byte[AuthenticationOptions.SigningKeyMinLength];
        SecurityKey key = new SymmetricSecurityKey(bytes);

        SigningCredentials signingKey =
            new(key, SecurityAlgorithms.HmacSha256);

        String signingKeyString = Convert.ToBase64String(bytes);
        this._accessTokenExpiry = TimeSpan.FromMinutes(2);
        this._accessTokenSigningKey = signingKey;
        this._accessTokenSigningKeyString = signingKeyString;
        this._audience = String.Empty;
        this._issuer = String.Empty;
        this._otpExpiry = TimeSpan.FromMinutes(10);
        this._refreshTokenExpiry = TimeSpan.FromDays(30);
        this._refreshTokenSigningKey = signingKey;
        this._refreshTokenSigningKeyString = signingKeyString;
    }
}
