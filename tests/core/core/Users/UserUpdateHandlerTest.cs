using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Utilities;

using Shipstone.Authenticator.Api.Core;
using Shipstone.Authenticator.Api.Core.Users;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

using Shipstone.Authenticator.Api.CoreTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.CoreTest.Users;

public sealed class UserUpdateHandlerTest
{
    private readonly MockClaimsService _claims;
    private readonly IUserUpdateHandler _handler;
    private readonly MockRepository _repository;

    public UserUpdateHandlerTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddAuthenticatorCore();
        MockClaimsService claims = new();
        services.AddSingleton<IClaimsService>(claims);
        MockRepository repository = new();
        services.AddSingleton<IRepository>(repository);
        IServiceProvider provider = new MockServiceProvider(services);
        this._claims = claims;
        this._handler = provider.GetRequiredService<IUserUpdateHandler>();
        this._repository = repository;
    }

#region HandleAsync method
#region Invalid arguments
    [Fact]
    public Task TestHandleAsync_Invalid_ForenameInvalid() => throw new NotImplementedException();

    [Fact]
    public async Task TestHandleAsync_Invalid_ForenameNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    null!,
                    "Doe",
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("forename", ex.ParamName);
    }

    [Fact]
    public Task TestHandleAsync_Invalid_SurnameInvalid() => throw new NotImplementedException();

    [Fact]
    public async Task TestHandleAsync_Invalid_SurnameNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    "John",
                    null!,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("surname", ex.ParamName);
    }
#endregion

#region Valid arguments
    [Fact]
    public Task TestHandleAsync_Valid_Failure_NotActive()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_GuidFunc = _ => new();
            return users;
        };

        this._claims._idFunc = Guid.NewGuid;

        // Act and assert
        return Assert.ThrowsAsync<UserNotActiveException>(() =>
            this._handler.HandleAsync("John", "Doe", CancellationToken.None));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_NotFound()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_GuidFunc = _ => null;
            return users;
        };

        this._claims._idFunc = Guid.NewGuid;

        // Act and assert
        return Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync("John", "Doe", CancellationToken.None));
    }

    [Fact]
    public async Task TestHandleAsync_Valid_Success()
    {
#region Arrange
        // Arrange
        Guid id = Guid.NewGuid();
        DateTime created = DateTime.UnixEpoch.ToUniversalTime();
        const String EMAIL_ADDRESS = "john.doe@contoso.com";
        const String FORENAME = "John";
        const String SURNAME = "Doe";

        DateOnly born =
            DateOnly
                .FromDateTime(DateTime.UtcNow.Date)
                .AddYears(-18);

        DateTime consented = created.AddDays(10);

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_GuidFunc = id =>
                new UserEntity
                {
                    Born = born,
                    Consented = consented,
                    Created = created,
                    EmailAddress = EMAIL_ADDRESS,
                    Id = id,
                    IsActive = true,
                    Updated = created
                };

            users._updateAction = _ => { };
            return users;
        };

        this._claims._idFunc = () => id;
        this._repository._saveAction = () => { };
        DateTime notBefore = DateTime.UtcNow;
#endregion

        // Act
        IUser user =
            await this._handler.HandleAsync(
                $" {FORENAME} ",
                $" {SURNAME} ",
                CancellationToken.None
            );

        // Assert
        Assert.False(DateTime.Compare(notBefore, user.Updated) > 0);

        user.AssertEqual(
            id,
            created,
            user.Updated,
            EMAIL_ADDRESS,
            FORENAME,
            SURNAME,
            born,
            consented
        );
    }
#endregion
#endregion
}
