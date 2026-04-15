using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Utilities;

using Shipstone.Authenticator.Api.Core;
using Shipstone.Authenticator.Api.Core.Accounts;
using Shipstone.Authenticator.Api.Infrastructure.Authentication;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;
using Shipstone.Authenticator.Api.Infrastructure.Mail;

using Shipstone.Authenticator.Api.CoreTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.CoreTest.Accounts;

public sealed class OtpAuthenticateHandlerTest
{
    private readonly MockAuthenticationService _authentication;
    private readonly IOtpAuthenticateHandler _handler;
    private readonly MockRepository _repository;

    public OtpAuthenticateHandlerTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddAuthenticatorCore();
        MockAuthenticationService authentication = new();
        services.AddSingleton<IAuthenticationService>(authentication);
        MockMailService mail = new();
        services.AddSingleton<IMailService>(mail);
        MockRepository repository = new();
        services.AddSingleton<IRepository>(repository);
        IServiceProvider provider = new MockServiceProvider(services);
        this._authentication = authentication;
        this._handler = provider.GetRequiredService<IOtpAuthenticateHandler>();
        this._repository = repository;
    }

#region HandleAsync method
    [Fact]
    public async Task TestHandleAsync_Invalid_EmailAddressNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    null!,
                    String.Empty,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("emailAddress", ex.ParamName);
    }

    [Fact]
    public async Task TestHandleAsync_Invalid_OtpNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    String.Empty,
                    null!,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("otp", ex.ParamName);
    }

#region Valid arguments
#region Failure
    [Fact]
    public Task TestHandleAsync_Valid_Failure_EmailAddressNotFound()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_StringFunc = _ => null;
            return users;
        };

        // Act and assert
        return Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                String.Empty,
                CancellationToken.None
            ));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_OtpExpired_OtpExpiresNotNull()
    {
        // Arrange
        const String OTP = "123456";

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = _ =>
                new UserEntity
                {
                    IsActive = true,
                    Otp = OTP,
                    OtpExpires = DateTime.MinValue
                };

            users._updateAction = _ => { };
            return users;
        };

        this._repository._saveAction = () => { };

        // Act and assert
        return Assert.ThrowsAsync<ForbiddenException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                OTP,
                CancellationToken.None
            ));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_OtpExpired_OtpExpiresNull()
    {
        // Arrange
        const String OTP = "123456";

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = _ =>
                new UserEntity
                {
                    IsActive = true,
                    Otp = OTP
                };

            users._updateAction = _ => { };
            return users;
        };

        this._repository._saveAction = () => { };

        // Act and assert
        return Assert.ThrowsAsync<ForbiddenException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                OTP,
                CancellationToken.None
            ));
    }

    [InlineData(null)]
    [InlineData("123456")]
    [Theory]
    public Task TestHandleAsync_Valid_Failure_OtpNotEqual(String? otp)
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = _ =>
                new UserEntity
                {
                    IsActive = true,
                    Otp = otp
                };

            return users;
        };

        // Act and assert
        return Assert.ThrowsAsync<ForbiddenException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                String.Empty,
                CancellationToken.None
            ));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotActive()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_StringFunc = _ => new();
            return users;
        };

        // Act and assert
        return Assert.ThrowsAsync<UserNotActiveException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                String.Empty,
                CancellationToken.None
            ));
    }
#endregion

    [Fact]
    public async Task TestHandleAsync_Valid_Success()
    {
#region Arrange
        // Arrange
        const String ACCESS_TOKEN = "My access token";
        const String REFRESH_TOKEN = "My refresh token";
        DateTime now = DateTime.UtcNow;
        DateTime refreshTokenExpires = now.AddDays(123456);
        const String OTP = "123456";

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = _ =>
                new UserEntity
                {
                    IsActive = true,
                    Otp = OTP,
                    OtpExpires = DateTime.MaxValue
                };

            users._updateAction = _ => { };
            return users;
        };

        this._repository._saveAction = () => { };

        this._authentication._authenticateAction = (_, _) =>
        {
            MockAuthenticateResult result = new();
            result._refreshTokenExpiresFunc = () => refreshTokenExpires;
            result._refreshTokenFunc = () => REFRESH_TOKEN;
            result._accessTokenFunc = () => ACCESS_TOKEN;
            return result;
        };

        this._repository._userRefreshTokensFunc = () =>
        {
            MockUserRefreshTokenRepository userRefreshTokens = new();
            userRefreshTokens._createAction = _ => { };
            return userRefreshTokens;
        };
#endregion

        // Act
        IAuthenticateResult result =
            await this._handler.HandleAsync(
                String.Empty,
                OTP,
                CancellationToken.None
            );

        // Assert
        Assert.Equal(ACCESS_TOKEN, result.AccessToken);
        Assert.Equal(REFRESH_TOKEN, result.RefreshToken);
        Assert.Equal(refreshTokenExpires, result.RefreshTokenExpires);
        Assert.Equal(DateTimeKind.Utc, result.RefreshTokenExpires.Kind);
    }
#endregion
#endregion
}
