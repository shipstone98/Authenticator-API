using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Xunit;

using Shipstone.Utilities;

using Shipstone.Authenticator.Api.Core.Accounts;
using Shipstone.Authenticator.Api.Infrastructure.Authentication;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

using Shipstone.Authenticator.Api.Infrastructure.AuthenticationTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.Infrastructure.AuthenticationTest;

public sealed class AuthenticationServiceTest
{
    private const String _audience = "My audience";
    private const int _otpExpiryMinutes = 123456789;

    private readonly IAuthenticationService _authentication;
    private readonly MockRandomNumberGenerator _rng;
    private readonly MockJwtSecurityTokenHandler _tokenHandler;

    public AuthenticationServiceTest()
    {
        IList<ServiceDescriptor> collection = new List<ServiceDescriptor>();
        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._countFunc = () => collection.Count;
        services._itemFunc = i => collection[i];
        services._getEnumeratorFunc = collection.GetEnumerator;

        services.AddAuthenticatorInfrastructureAuthentication(options =>
        {
            options.Audiences =
                new String[1] { AuthenticationServiceTest._audience };

            options.OtpExpiryMinutes =
                AuthenticationServiceTest._otpExpiryMinutes;
        });

        MockRandomNumberGenerator rng = new();
        services.AddSingleton<RandomNumberGenerator>(rng);
        MockJwtSecurityTokenHandler tokenHandler = new();
        services.AddSingleton<JwtSecurityTokenHandler>(tokenHandler);
        IServiceProvider provider = new MockServiceProvider(services);

        this._authentication =
            provider.GetRequiredService<IAuthenticationService>();

        this._rng = rng;
        this._tokenHandler = tokenHandler;
    }

#region AuthenticateAsync method
    [Fact]
    public async Task TestAuthenticateAsync_Invalid_AudienceNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._authentication.AuthenticateAsync(
                    null!,
                    new UserEntity { },
                    DateTime.UtcNow,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("audience", ex.ParamName);
    }

    [Fact]
    public async Task TestAuthenticateAsync_Invalid_UserNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._authentication.AuthenticateAsync(
                    String.Empty,
                    null!,
                    DateTime.UtcNow,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("user", ex.ParamName);
    }

    [Fact]
    public Task TestAuthenticateAsync_Valid_Failure()
    {
        // Arrange
        const String TOKEN = "My token";
        DateTime now = DateTime.UtcNow;
        this._tokenHandler._createTokenFunc = _ => new MockSecurityToken();
        this._tokenHandler._writeTokenFunc = _ => TOKEN;

        // Act
        return Assert.ThrowsAsync<ForbiddenException>(() =>
            this._authentication.AuthenticateAsync(
                String.Empty,
                new UserEntity { },
                now,
                CancellationToken.None
            ));
    }

    [Fact]
    public async Task TestAuthenticateAsync_Valid_Success()
    {
        // Arrange
        const String TOKEN = "My token";
        DateTime now = DateTime.UtcNow;
        this._tokenHandler._createTokenFunc = _ => new MockSecurityToken();
        this._tokenHandler._writeTokenFunc = _ => TOKEN;

        // Act
        IAuthenticateResult result =
            await this._authentication.AuthenticateAsync(
                AuthenticationServiceTest._audience,
                new UserEntity { },
                now,
                CancellationToken.None
            );

        // Assert
        Assert.Equal(TOKEN, result.AccessToken);
        Assert.Equal(TOKEN, result.RefreshToken);
        Assert.False(DateTime.Equals(now, result.RefreshTokenExpires));
        Assert.Equal(DateTimeKind.Utc, result.RefreshTokenExpires.Kind);
    }
#endregion

    [Fact]
    public async Task TestGenerateOtpAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._authentication.GenerateOtpAsync(
                    null!,
                    DateTime.MinValue,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("user", ex.ParamName);
    }

    [Fact]
    public async Task TestGenerateOtpAsync_Valid()
    {
        // Arrange
        UserEntity user = new();
        DateTime now = DateTime.UtcNow;
        this._rng._getNonZeroBytesAction = _ => { };

        // Act
        await this._authentication.GenerateOtpAsync(
            user,
            now,
            CancellationToken.None
        );

        // Assert
        int otp = Int32.Parse(user.Otp!);
        Assert.False(otp < 100_000);
        Assert.True(otp < 1_000_000);

        Assert.Equal(
            now.AddMinutes(AuthenticationServiceTest._otpExpiryMinutes),
            user.OtpExpires!.Value
        );

        Assert.Equal(DateTimeKind.Utc, user.OtpExpires.Value.Kind);
        Assert.Equal(now, user.Updated);
        Assert.Equal(DateTimeKind.Utc, user.Updated.Kind);
    }

#region GetPropertiesAsync method
    [Fact]
    public async Task TestGetPropertiesAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._authentication.GetPropertiesAsync(
                    null!,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("token", ex.ParamName);
    }

#region Valid arguments
#region Failure
    [Fact]
    public Task TestGetPropertiesAsync_Valid_Failure_TokenNotValid()
    {
        // Arrange
        this._tokenHandler._validateTokenFunc = (_, _) => new();

        // Act and assert
        return Assert.ThrowsAsync<UnauthorizedException>(() =>
            this._authentication.GetPropertiesAsync(
                String.Empty,
                CancellationToken.None
            ));
    }

#region Token valid
    [Fact]
    public Task TestGetPropertiesAsync_Valid_Failure_TokenValid_IdNotValid()
    {
        // Arrange
        this._tokenHandler._validateTokenFunc = (_, _) =>
        {
            TokenValidationResult result = new TokenValidationResult();
            Claim audience = new(JwtRegisteredClaimNames.Aud, "My audience");
            Claim subject = new(JwtRegisteredClaimNames.Sub, "12345");
            IEnumerable<Claim> claims = new Claim[2] { audience, subject };
            result.ClaimsIdentity = new(claims);
            result.IsValid = true;
            return result;
        };

        // Act and assert
        return Assert.ThrowsAsync<UnauthorizedException>(() =>
            this._authentication.GetPropertiesAsync(
                String.Empty,
                CancellationToken.None
            ));
    }

    [Fact]
    public Task TestGetPropertiesAsync_Valid_Failure_TokenValid_NotContainsAudience()
    {
        // Arrange
        this._tokenHandler._validateTokenFunc = (_, _) =>
        {
            TokenValidationResult result = new TokenValidationResult();
            Claim subject = new(JwtRegisteredClaimNames.Sub, "12345");
            IEnumerable<Claim> claims = new Claim[1] { subject };
            result.ClaimsIdentity = new(claims);
            result.IsValid = true;
            return result;
        };

        // Act and assert
        return Assert.ThrowsAsync<UnauthorizedException>(() =>
            this._authentication.GetPropertiesAsync(
                String.Empty,
                CancellationToken.None
            ));
    }

    [Fact]
    public Task TestGetPropertiesAsync_Valid_Failure_TokenValid_NotContainsId()
    {
        // Arrange
        this._tokenHandler._validateTokenFunc = (_, _) =>
        {
            TokenValidationResult result = new TokenValidationResult();
            Claim audience = new(JwtRegisteredClaimNames.Aud, "My audience");
            IEnumerable<Claim> claims = new Claim[1] { audience };
            result.ClaimsIdentity = new(claims);
            result.IsValid = true;
            return result;
        };

        // Act and assert
        return Assert.ThrowsAsync<UnauthorizedException>(() =>
            this._authentication.GetPropertiesAsync(
                String.Empty,
                CancellationToken.None
            ));
    }
#endregion
#endregion

    [Fact]
    public async Task TestGetPropertiesAsync_Valid_Success()
    {
        // Arrange
        const String AUDIENCE = "My audience";
        Guid id = Guid.NewGuid();

        this._tokenHandler._validateTokenFunc = (_, _) =>
        {
            TokenValidationResult result = new TokenValidationResult();
            String idString = id.ToString();
            Claim audience = new(JwtRegisteredClaimNames.Aud, AUDIENCE);
            Claim subject = new(JwtRegisteredClaimNames.Sub, idString);
            IEnumerable<Claim> claims = new Claim[2] { audience, subject };
            result.ClaimsIdentity = new(claims);
            result.IsValid = true;
            return result;
        };

        // Act
        (String Audience, Guid Id) result =
            await this._authentication.GetPropertiesAsync(
                String.Empty,
                CancellationToken.None
            );

        // Assert
        Assert.Equal(AUDIENCE, result.Audience);
        Assert.Equal(id, result.Id);
    }
#endregion
#endregion
}
