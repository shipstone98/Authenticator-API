using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Authenticator.Api.Core.Accounts;
using Shipstone.Authenticator.Api.Infrastructure.Authentication;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

using Shipstone.Authenticator.Api.Infrastructure.AuthenticationTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.Infrastructure.AuthenticationTest;

public sealed class AuthenticationServiceTest
{
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
            options.OtpExpiryMinutes = AuthenticationServiceTest._otpExpiryMinutes);

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

    [Fact]
    public async Task TestAuthenticateAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._authentication.AuthenticateAsync(
                    null!,
                    DateTime.UtcNow,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("user", ex.ParamName);
    }

    [Fact]
    public async Task TestAuthenticateAsync_Valid()
    {
        // Arrange
        const String TOKEN = "My token";
        DateTime now = DateTime.UtcNow;
        this._tokenHandler._createTokenFunc = _ => new MockSecurityToken();
        this._tokenHandler._writeTokenFunc = _ => TOKEN;

        // Act
        IAuthenticateResult result =
            await this._authentication.AuthenticateAsync(
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

#region GetId method
#region Invalid arguments
    [Fact]
    public void TestGetId_Invalid_IdInvalid()
    {
        // Arrange
        this._tokenHandler._validateTokenFunc = (_, _) =>
        {
            ClaimsIdentity identity = new();
            Claim claim = new(ClaimTypes.NameIdentifier, "My ID");
            identity.AddClaim(claim);
            ClaimsPrincipal principal = new(identity);
            return (principal, null!);
        };

        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentException>(() =>
                this._authentication.GetId(String.Empty));

        // Assert
        Assert.Equal("token", ex.ParamName);
    }

    [Fact]
    public void TestGetId_Invalid_NotContainsId()
    {
        // Arrange
        this._tokenHandler._validateTokenFunc = (_, _) =>
        {
            ClaimsPrincipal principal = new();
            return (principal, null!);
        };

        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentException>(() =>
                this._authentication.GetId(String.Empty));

        // Assert
        Assert.Equal("token", ex.ParamName);
    }

    [Fact]
    public void TestGetId_Invalid_Null()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                this._authentication.GetId(null!));

        // Assert
        Assert.Equal("token", ex.ParamName);
    }
#endregion

    [Fact]
    public void TestGetId_Valid()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        this._tokenHandler._validateTokenFunc = (_, _) =>
        {
            ClaimsIdentity identity = new();
            String idString = id.ToString();
            Claim claim = new(ClaimTypes.NameIdentifier, idString);
            identity.AddClaim(claim);
            ClaimsPrincipal principal = new(identity);
            return (principal, null!);
        };

        // Act
        Guid result = this._authentication.GetId(String.Empty);

        // Assert
        Assert.Equal(id, result);
    }
#endregion
}
