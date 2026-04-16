using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

using Shipstone.Extensions.Security;

using Shipstone.Authenticator.Api.Infrastructure.Data;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

using Shipstone.Authenticator.Api.Infrastructure.DataTest.Mocks;
using Shipstone.Authenticator.Api.Test.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.Infrastructure.DataTest.Repositories;

public sealed class UserRepositoryTest
{
    private readonly MockDataSource _dataSource;
    private readonly MockNormalizationService _normalization;
    private readonly IUserRepository _repository;

    public UserRepositoryTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddAuthenticatorInfrastructureData();
        MockDataSource dataSource = new();
        services.AddSingleton<IDataSource>(dataSource);
        MockNormalizationService normalization = new();
        services.AddSingleton<INormalizationService>(normalization);
        MockOptions<SecurityOptions> securityOptions = new();
        services.AddSingleton<IOptions<SecurityOptions>>(securityOptions);
        securityOptions._valueFunc = () => new();
        IServiceProvider provider = new MockServiceProvider(services);
        this._dataSource = dataSource;
        this._normalization = normalization;
        this._repository = provider.GetRequiredService<IUserRepository>();
    }

    [Fact]
    public async Task TestCreateAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._repository.CreateAsync(
                    null!,
                    TestContext.Current.CancellationToken
                ));

        // Assert
        Assert.Equal("user", ex.ParamName);
    }

    [Fact]
    public Task TestCreateAsync_Valid()
    {
        // Arrange
        UserEntity user = new();

        this._dataSource._usersFunc = () =>
        {
            IQueryable<UserEntity> query =
                Array
                    .Empty<UserEntity>()
                    .AsQueryable();

            MockDataSet<UserEntity> dataSet = new(query);
            dataSet._setStateAction = (_, _) => { };
            return dataSet;
        };

        // Act
        return this._repository.CreateAsync(
            user,
            TestContext.Current.CancellationToken
        );

        // Nothing to assert
    }

#region RetrieveAsync methods
#region Guid parameter
    [Fact]
    public async Task TestRetrieveAsync_Guid_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentException>(() =>
                this._repository.RetrieveAsync(
                    Guid.Empty,
                    TestContext.Current.CancellationToken
                ));

        // Assert
        Assert.Equal("id", ex.ParamName);
    }

    [Fact]
    public async Task TestRetrieveAsync_Guid_Valid_Contains()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        this._dataSource._usersFunc = () =>
        {
            IEnumerable<UserEntity> users = new UserEntity[]
            {
                new UserEntity
                {
                    Id = id
                }
            };

            IQueryable<UserEntity> query = users.AsQueryable();
            return new MockDataSet<UserEntity>(query);
        };

        // Act
        UserEntity? user =
            await this._repository.RetrieveAsync(
                id,
                TestContext.Current.CancellationToken
            );

        // Assert
        Assert.NotNull(user);
        Assert.Equal(id, user.Id);
    }

    [Fact]
    public async Task TestRetrieveAsync_Guid_Valid_NotContains()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        this._dataSource._usersFunc = () =>
        {
            IQueryable<UserEntity> query =
                Array
                    .Empty<UserEntity>()
                    .AsQueryable();

            return new MockDataSet<UserEntity>(query);
        };

        // Act
        UserEntity? user =
            await this._repository.RetrieveAsync(
                id,
                TestContext.Current.CancellationToken
            );

        // Assert
        Assert.Null(user);
    }
#endregion

#region String parameter
    [Fact]
    public async Task TestRetrieveAsync_String_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._repository.RetrieveAsync(
                    null!,
                    TestContext.Current.CancellationToken
                ));

        // Assert
        Assert.Equal("emailAddress", ex.ParamName);
    }

    [Fact]
    public async Task TestRetrieveAsync_String_Valid_Contains()
    {
        // Arrange
        const String EMAIL_ADDRESS_NORMALIZED = "JOHN.DOE@CONTOSO.COM";
        const String EMAIL_ADDRESS = "john.doe@contoso.com";

        this._normalization._normalizeFunc = _ => EMAIL_ADDRESS_NORMALIZED;

        this._dataSource._usersFunc = () =>
        {
            IEnumerable<UserEntity> users = new UserEntity[]
            {
                new UserEntity
                {
                    EmailAddress = EMAIL_ADDRESS,
                    EmailAddressNormalized = EMAIL_ADDRESS_NORMALIZED
                }
            };

            IQueryable<UserEntity> query = users.AsQueryable();
            return new MockDataSet<UserEntity>(query);
        };

        // Act
        UserEntity? user =
            await this._repository.RetrieveAsync(
                EMAIL_ADDRESS,
                TestContext.Current.CancellationToken
            );

        // Assert
        Assert.NotNull(user);
        Assert.Equal(EMAIL_ADDRESS, user.EmailAddress);
    }

    [Fact]
    public async Task TestRetrieveAsync_String_Valid_NotContains()
    {
        // Arrange
        this._normalization._normalizeFunc = _ => String.Empty;

        this._dataSource._usersFunc = () =>
        {
            IQueryable<UserEntity> query =
                Array
                    .Empty<UserEntity>()
                    .AsQueryable();

            return new MockDataSet<UserEntity>(query);
        };

        // Act
        UserEntity? user =
            await this._repository.RetrieveAsync(
                "john.doe@contoso.com",
                TestContext.Current.CancellationToken
            );

        // Assert
        Assert.Null(user);
    }
#endregion
#endregion

    [Fact]
    public async Task TestUpdateAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._repository.UpdateAsync(
                    null!,
                    TestContext.Current.CancellationToken
                ));

        // Assert
        Assert.Equal("user", ex.ParamName);
    }

    [Fact]
    public Task TestUpdateAsync_Valid()
    {
        // Arrange
        UserEntity user = new();

        this._dataSource._usersFunc = () =>
        {
            IQueryable<UserEntity> query =
                Array
                    .Empty<UserEntity>()
                    .AsQueryable();

            MockDataSet<UserEntity> dataSet = new(query);
            dataSet._setStateAction = (_, _) => { };
            return dataSet;
        };

        // Act
        return this._repository.UpdateAsync(
            user,
            TestContext.Current.CancellationToken
        );

        // Nothing to assert
    }
}
