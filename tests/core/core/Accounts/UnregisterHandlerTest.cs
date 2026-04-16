using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Utilities;

using Shipstone.Authenticator.Api.Core;
using Shipstone.Authenticator.Api.Core.Accounts;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;
using Shipstone.Authenticator.Api.Infrastructure.Mail;

using Shipstone.Authenticator.Api.CoreTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.CoreTest.Accounts;

public sealed class UnregisterHandlerTest
{
    private readonly MockClaimsService _claims;
    private readonly IUnregisterHandler _handler;
    private readonly MockMailService _mail;
    private readonly MockRepository _repository;

    public UnregisterHandlerTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddAuthenticatorCore();
        MockClaimsService claims = new();
        services.AddSingleton<IClaimsService>(claims);
        MockMailService mail = new();
        services.AddSingleton<IMailService>(mail);
        MockRepository repository = new();
        services.AddSingleton<IRepository>(repository);
        IServiceProvider provider = new MockServiceProvider(services);
        this._claims = claims;
        this._handler = provider.GetRequiredService<IUnregisterHandler>();
        this._mail = mail;
        this._repository = repository;
    }

#region HandleAsync method
#region Failure
    [Fact]
    public Task TestHandleAsync_Failure_UserNotActive()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_GuidFunc = _ => new();
            return users;
        };

        this._claims._idFunc = Guid.NewGuid;

        // Act
        return Assert.ThrowsAsync<UserNotActiveException>(() =>
            this._handler.HandleAsync(TestContext.Current.CancellationToken));

        // Nothing to assert
    }

    [Fact]
    public async Task TestHandleAsync_Failure_UserNotAuthenticated()
    {
        // Arrange
        Exception innerException = new UnauthorizedException();
        this._repository._usersFunc = () => new MockUserRepository();
        this._claims._idFunc = () => throw innerException;

        // Act
        Exception ex =
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                this._handler.HandleAsync(TestContext.Current.CancellationToken));

        // Assert
        Assert.Same(innerException, ex);
    }

    [Fact]
    public Task TestHandleAsync_Failure_UserNotFound()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_GuidFunc = _ => null;
            return users;
        };

        this._claims._idFunc = Guid.NewGuid;

        // Act
        return Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(TestContext.Current.CancellationToken));

        // Nothing to assert
    }
#endregion

    [Fact]
    public Task TestHandleAsync_Success()
    {
#region Arrange
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_GuidFunc = _ =>
                new UserEntity
                {
                    IsActive = true
                };

            users._updateAction = _ => { };
            return users;
        };

        this._claims._idFunc = Guid.NewGuid;

        this._repository._userRefreshTokensFunc = () =>
        {
            MockUserRefreshTokenRepository userRefreshTokens = new();

            userRefreshTokens._listForUserFunc = _ =>
                new UserRefreshTokenEntity[]
                {
                    new UserRefreshTokenEntity { }
                };

            userRefreshTokens._deleteAction = _ => { };
            return userRefreshTokens;
        };

        this._repository._saveAction = () => { };
        this._claims._emailAddressFunc = () => String.Empty;
        this._mail._sendUnregistrationAction = _ => { };
#endregion

        // Act
        return this._handler.HandleAsync(TestContext.Current.CancellationToken);

        // Nothing to assert
    }
#endregion
}
