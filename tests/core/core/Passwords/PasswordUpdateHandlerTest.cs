using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Extensions.Identity;
using Shipstone.Utilities;

using Shipstone.Authenticator.Api.Core;
using Shipstone.Authenticator.Api.Core.Accounts;
using Shipstone.Authenticator.Api.Core.Passwords;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

using Shipstone.Authenticator.Api.CoreTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.CoreTest.Passwords;

public sealed class PasswordUpdateHandlerTest
{
    private readonly MockClaimsService _claims;
    private readonly IPasswordUpdateHandler _handler;
    private readonly MockPasswordService _password;
    private readonly MockRepository _repository;

    public PasswordUpdateHandlerTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddAuthenticatorCore();
        MockClaimsService claims = new();
        services.AddSingleton<IClaimsService>(claims);
        MockPasswordService password = new();
        services.AddSingleton<IPasswordService>(password);
        MockRepository repository = new();
        services.AddSingleton<IRepository>(repository);
        IServiceProvider provider = new MockServiceProvider(services);
        this._claims = claims;
        this._handler = provider.GetRequiredService<IPasswordUpdateHandler>();
        this._password = password;
        this._repository = repository;
    }

#region HandleAsync method
    [Fact]
    public async Task TestHandleAsync_Invalid_PasswordCurrentNull()
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
        Assert.Equal("passwordCurrent", ex.ParamName);
    }

    [Fact]
    public async Task TestHandleAsync_Invalid_PasswordNewInvalid()
    {
        // Arrange
        ArgumentException innerException = new();
        this._password._validateAction = _ => throw innerException;

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentException>(() =>
                this._handler.HandleAsync(
                    String.Empty,
                    null!,
                    CancellationToken.None
                ));

        // Assert
        Assert.Same(innerException, ex);
    }

#region Valid arguments
#region Failure
    [Fact]
    public async Task TestHandleAsync_Valid_Failure_PasswordNotCorrect()
    {
        // Arrange
        IncorrectPasswordException innerException = new();
        this._password._validateAction = _ => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_GuidFunc = _ =>
                new UserEntity
                {
                    IsActive = true,
                    PasswordHash = String.Empty
                };

            return users;
        };

        this._claims._idFunc = Guid.NewGuid;
        this._password._verifyFunc = (_, _) => throw innerException;

        // Act
        Exception ex =
            await Assert.ThrowsAsync<IncorrectPasswordException>(() =>
                this._handler.HandleAsync(
                    String.Empty,
                    String.Empty,
                    CancellationToken.None
                ));

        // Assert
        Assert.Same(innerException, ex.InnerException);
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotActive()
    {
        // Arrange
        this._password._validateAction = _ => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_GuidFunc = _ => new();
            return users;
        };

        this._claims._idFunc = Guid.NewGuid;

        // Act and assert
        return Assert.ThrowsAsync<UserNotActiveException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                String.Empty,
                CancellationToken.None
            ));
    }

    [Fact]
    public async Task TestHandleAsync_Valid_Failure_UserNotAuthenticated()
    {
        // Arrange
        UnauthorizedException innerException = new();
        this._password._validateAction = _ => { };
        this._repository._usersFunc = () => new MockUserRepository();
        this._claims._idFunc = () => throw innerException;

        // Act
        Exception ex =
            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                this._handler.HandleAsync(
                    String.Empty,
                    String.Empty,
                    CancellationToken.None
                ));

        // Assert
        Assert.Same(innerException, ex);
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotFound()
    {
        // Arrange
        this._password._validateAction = _ => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_GuidFunc = _ => null;
            return users;
        };

        this._claims._idFunc = Guid.NewGuid;

        // Act and assert
        return Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                String.Empty,
                CancellationToken.None
            ));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotVerified()
    {
        // Arrange
        this._password._validateAction = _ => { };

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

        this._claims._idFunc = Guid.NewGuid;

        // Act and assert
        return Assert.ThrowsAsync<ForbiddenException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                String.Empty,
                CancellationToken.None
            ));
    }
#endregion

    [Fact]
    public Task TestHandleAsync_Valid_Success()
    {
        // Arrange
        this._password._validateAction = _ => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_GuidFunc = _ =>
                new UserEntity
                {
                    IsActive = true,
                    PasswordHash = String.Empty
                };

            users._updateAction = _ => { };
            return users;
        };

        this._claims._idFunc = Guid.NewGuid;
        this._password._verifyFunc = (_, _) => false;
        this._password._hashFunc = _ => String.Empty;
        this._repository._saveAction = () => { };

        // Act and assert
        return this._handler.HandleAsync(
            String.Empty,
            String.Empty,
            CancellationToken.None
        );
    }
#endregion
#endregion
}
