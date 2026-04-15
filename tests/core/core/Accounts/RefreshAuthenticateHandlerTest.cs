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

using Shipstone.Authenticator.Api.CoreTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.CoreTest.Accounts;

public sealed class RefreshAuthenticateHandlerTest
{
    private readonly MockAuthenticationService _authentication;
    private readonly IRefreshAuthenticateHandler _handler;
    private readonly MockRepository _repository;

    public RefreshAuthenticateHandlerTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddAuthenticatorCore();
        MockAuthenticationService authentication = new();
        services.AddSingleton<IAuthenticationService>(authentication);
        MockRepository repository = new();
        services.AddSingleton<IRepository>(repository);
        IServiceProvider provider = new MockServiceProvider(services);
        this._authentication = authentication;

        this._handler =
            provider.GetRequiredService<IRefreshAuthenticateHandler>();

        this._repository = repository;
    }

#region HandleAsync method
    [Fact]
    public async Task TestHandleAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(null!, CancellationToken.None));

        // Assert
        Assert.Equal("refreshToken", ex.ParamName);
    }

#region Valid arguments
#region Failure
    [Fact]
    public async Task TestHandleAsync_Valid_Failure_RefreshTokenNotValid()
    {
        // Arrange
        Exception innerException = new ArgumentException();
        this._authentication._getPropertiesFunc = _ => throw innerException;

        // Act
        Exception ex =
            await Assert.ThrowsAsync<ForbiddenException>(() =>
                this._handler.HandleAsync(
                    String.Empty,
                    CancellationToken.None
                ));

        // Assert
        Assert.Same(innerException, ex.InnerException);
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserRefreshTokenExpired()
    {
        // Arrange
        this._authentication._getPropertiesFunc = _ =>
            (String.Empty, Guid.Empty);

        this._repository._userRefreshTokensFunc = () =>
        {
            MockUserRefreshTokenRepository userRefreshTokens = new();

            userRefreshTokens._retrieveFunc = _ =>
                new UserRefreshTokenEntity
                {
                    Expires = DateTime.MinValue
                };

            userRefreshTokens._deleteAction = _ => { };
            return userRefreshTokens;
        };

        this._repository._saveAction = () => { };

        // Act and assert
        return Assert.ThrowsAsync<ForbiddenException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                CancellationToken.None
            ));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserRefreshTokenNotFound()
    {
        // Arrange
        this._authentication._getPropertiesFunc = _ =>
            (String.Empty, Guid.Empty);

        this._repository._userRefreshTokensFunc = () =>
        {
            MockUserRefreshTokenRepository userRefreshTokens = new();
            userRefreshTokens._retrieveFunc = _ => null;
            return userRefreshTokens;
        };

        // Act and assert
        return Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                CancellationToken.None
            ));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotActive()
    {
        // Arrange
        this._authentication._getPropertiesFunc = _ =>
            (String.Empty, Guid.Empty);

        this._repository._userRefreshTokensFunc = () =>
        {
            MockUserRefreshTokenRepository userRefreshTokens = new();

            userRefreshTokens._retrieveFunc = _ =>
                new UserRefreshTokenEntity
                {
                    Expires = DateTime.MaxValue
                };

            userRefreshTokens._deleteAction = _ => { };
            return userRefreshTokens;
        };

        this._repository._saveAction = () => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_GuidFunc = _ => new();
            return users;
        };

        // Act and assert
        return Assert.ThrowsAsync<UserNotActiveException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                CancellationToken.None
            ));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotFound()
    {
        // Arrange
        this._authentication._getPropertiesFunc = _ =>
            (String.Empty, Guid.Empty);

        this._repository._userRefreshTokensFunc = () =>
        {
            MockUserRefreshTokenRepository userRefreshTokens = new();

            userRefreshTokens._retrieveFunc = _ =>
                new UserRefreshTokenEntity
                {
                    Expires = DateTime.MaxValue
                };

            userRefreshTokens._deleteAction = _ => { };
            return userRefreshTokens;
        };

        this._repository._saveAction = () => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_GuidFunc = _ => null;
            return users;
        };

        // Act and assert
        return Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(
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

        this._authentication._getPropertiesFunc = _ =>
            (String.Empty, Guid.Empty);

        this._repository._userRefreshTokensFunc = () =>
        {
            MockUserRefreshTokenRepository userRefreshTokens = new();

            userRefreshTokens._retrieveFunc = _ =>
                new UserRefreshTokenEntity
                {
                    Expires = DateTime.MaxValue
                };

            userRefreshTokens._deleteAction = _ => { };
            userRefreshTokens._createAction = _ => { };
            return userRefreshTokens;
        };

        this._repository._saveAction = () => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_GuidFunc = _ =>
                new UserEntity
                {
                    IsActive = true
                };

            return users;
        };

        this._authentication._authenticateAction = (_, _, _) =>
        {
            MockAuthenticateResult result = new();
            result._refreshTokenExpiresFunc = () => refreshTokenExpires;
            result._refreshTokenFunc = () => REFRESH_TOKEN;
            result._accessTokenFunc = () => ACCESS_TOKEN;
            return result;
        };
#endregion

        // Act
        IAuthenticateResult result =
            await this._handler.HandleAsync(
                String.Empty,
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
