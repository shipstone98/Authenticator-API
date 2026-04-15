using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Shipstone.Utilities;
using Shipstone.Utilities.Security.Cryptography;

using Shipstone.Authenticator.Api.Core;
using Shipstone.Authenticator.Api.Core.Accounts;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Infrastructure.Authentication;

internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly AuthenticationOptions _options;
    private readonly RandomNumberGenerator _rng;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public AuthenticationService(
        RandomNumberGenerator rng,
        JwtSecurityTokenHandler tokenHandler,
        IOptions<AuthenticationOptions>? options
    )
    {
        ArgumentNullException.ThrowIfNull(rng);
        ArgumentNullException.ThrowIfNull(tokenHandler);
        AuthenticationOptions optionsValue = options?.Value ?? new();
        this._options = optionsValue;
        this._rng = rng;
        this._tokenHandler = tokenHandler;
    }

    private String GenerateAccessToken(
        String audience,
        UserEntity user,
        DateTime now
    )
    {
        String id = user.Id.ToString();
        String emailAddress = user.EmailAddress;

        IEnumerable<Claim> claims = new List<Claim>
        {
            new(ClaimTypes.Email, emailAddress),
            new(ClaimTypes.GivenName, user.Forename),
            new(ClaimTypes.Name, emailAddress),
            new(ClaimTypes.NameIdentifier, id),
            new(ClaimTypes.Surname, user.Surname)
        };

        SecurityToken token =
            this._tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Audience = audience,
                Expires = now.Add(this._options._accessTokenExpiry),
                IssuedAt = now,
                Issuer = this._options._issuer,
                NotBefore = now,
                SigningCredentials = this._options._accessTokenSigningKey,
                Subject = new(claims)
            });

        return this._tokenHandler.WriteToken(token);
    }

    private (String Value, DateTime Expires) GenerateRefreshToken(
        String audience,
        UserEntity user,
        DateTime now
    )
    {
        String id = user.Id.ToString();

        IEnumerable<Claim> claims =
            new Claim[] { new(ClaimTypes.NameIdentifier, id) };

        DateTime expires = now.Add(this._options._refreshTokenExpiry);

        SecurityToken token =
            this._tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Audience = audience,
                Expires = expires,
                IssuedAt = now,
                Issuer = this._options._issuer,
                NotBefore = now,
                SigningCredentials = this._options._refreshTokenSigningKey,
                Subject = new(claims)
            });

        String val = this._tokenHandler.WriteToken(token);
        return (val, expires);
    }

    private async Task<(String Audience, Guid Id)> GetPropertiesAsync(String token)
    {
        TokenValidationResult result =
            await this._tokenHandler.ValidateTokenAsync(
                token,
                new TokenValidationParameters
                {
                    IssuerSigningKey =
                        this._options._refreshTokenSigningKey.Key,
                    ValidIssuer = this._options._issuer,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true
                }
            );

        if (!result.IsValid)
        {
            throw new UnauthorizedException(
                "The provided token could not be verified.",
                result.Exception
            );
        }

        IEnumerable<Claim> claims = result.ClaimsIdentity.Claims;

        Claim? audience =
            claims.FirstOrDefault(c =>
                c.Type.Equals(JwtRegisteredClaimNames.Aud));

        Claim? subject =
            claims.FirstOrDefault(c =>
                c.Type.Equals(JwtRegisteredClaimNames.Sub));

        if (
            audience is null
            || subject is null
            || !Guid.TryParse(subject.Value, out Guid id)
        )
        {
            throw new UnauthorizedException("The provided token is not a valid token.");
        }

        return (audience.Value, id);
    }

    Task<IAuthenticateResult> IAuthenticationService.AuthenticateAsync(
        String audience,
        UserEntity user,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(audience);
        ArgumentNullException.ThrowIfNull(user);

        if (!this._options._audiences.Contains(audience))
        {
            throw new ForbiddenException("The provided audience is not authorized.");
        }

        String accessToken = this.GenerateAccessToken(audience, user, now);

        (String refreshToken, DateTime refreshTokenExpires) =
            this.GenerateRefreshToken(audience, user, now);

        IAuthenticateResult result =
            new AuthenticateResult(
                accessToken,
                refreshToken,
                refreshTokenExpires
            );

        return Task.FromResult(result);
    }

    Task IAuthenticationService.GenerateOtpAsync(
        UserEntity user,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(user);
        String otp = this._rng.GetNonZeroString(Constants.UserOtpMaxLength);
        DateTime otpExpires = now.Add(this._options._otpExpiry);
        user.Otp = otp;
        user.OtpExpires = otpExpires;
        user.Updated = now;
        return Task.CompletedTask;
    }

    Task<(String Audience, Guid Id)> IAuthenticationService.GetPropertiesAsync(
        String token,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(token);
        return this.GetPropertiesAsync(token);
    }
}
