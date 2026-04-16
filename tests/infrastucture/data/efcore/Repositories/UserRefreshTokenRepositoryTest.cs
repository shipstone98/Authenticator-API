using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Authenticator.Api.Infrastructure.Data;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

using Shipstone.Authenticator.Api.Infrastructure.DataTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.Infrastructure.DataTest.Repositories;

public sealed class UserRefreshTokenRepositoryTest
{
    private readonly MockDataSource _dataSource;
    private readonly IUserRefreshTokenRepository _repository;

    public UserRefreshTokenRepositoryTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddAuthenticatorInfrastructureData();
        MockDataSource dataSource = new();
        services.AddSingleton<IDataSource>(dataSource);
        IServiceProvider provider = new MockServiceProvider(services);
        this._dataSource = dataSource;

        this._repository =
            provider.GetRequiredService<IUserRefreshTokenRepository>();
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
        Assert.Equal("userRefreshToken", ex.ParamName);
    }

    [Fact]
    public Task TestCreateAsync_Valid()
    {
        // Arrange
        UserRefreshTokenEntity userRefreshToken = new();

        this._dataSource._userRefreshTokensFunc = () =>
        {
            IQueryable<UserRefreshTokenEntity> query =
                Array
                    .Empty<UserRefreshTokenEntity>()
                    .AsQueryable();

            MockDataSet<UserRefreshTokenEntity> dataSet = new(query);
            dataSet._setStateAction = (_, _) => { };
            return dataSet;
        };

        // Act
        return this._repository.CreateAsync(
            userRefreshToken,
            TestContext.Current.CancellationToken
        );

        // Nothing to assert
    }

    [Fact]
    public async Task TestDeleteAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._repository.DeleteAsync(
                    null!,
                    TestContext.Current.CancellationToken
                ));

        // Assert
        Assert.Equal("userRefreshToken", ex.ParamName);
    }

    [Fact]
    public Task TestDeleteAsync_Valid()
    {
        // Arrange
        UserRefreshTokenEntity userRefreshToken = new();

        this._dataSource._userRefreshTokensFunc = () =>
        {
            IQueryable<UserRefreshTokenEntity> query =
                Array
                    .Empty<UserRefreshTokenEntity>()
                    .AsQueryable();

            MockDataSet<UserRefreshTokenEntity> dataSet = new(query);
            dataSet._setStateAction = (_, _) => { };
            return dataSet;
        };

        // Act
        return this._repository.DeleteAsync(
            userRefreshToken,
            TestContext.Current.CancellationToken
        );

        // Nothing to assert
    }

#region RetrieveAsync method
    [Fact]
    public async Task TestRetrieveAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._repository.RetrieveAsync(
                    null!,
                    TestContext.Current.CancellationToken
                ));

        // Assert
        Assert.Equal("val", ex.ParamName);
    }

    [InlineData("")]
    [InlineData(" ")]
    [InlineData("My refresh token")]
    [Theory]
    public async Task TestRetrieveAsync_Valid_Contains(String val)
    {
        // Arrange
        this._dataSource._userRefreshTokensFunc = () =>
        {
            IEnumerable<UserRefreshTokenEntity> userRefreshTokens =
                new UserRefreshTokenEntity[]
            {
                new UserRefreshTokenEntity
                {
                    Value = val
                }
            };

            IQueryable<UserRefreshTokenEntity> query =
                userRefreshTokens.AsQueryable();

            return new MockDataSet<UserRefreshTokenEntity>(query);
        };

        // Act
        UserRefreshTokenEntity? userRefreshToken =
            await this._repository.RetrieveAsync(
                val,
                TestContext.Current.CancellationToken
            );

        // Assert
        Assert.NotNull(userRefreshToken);
        Assert.Equal(val, userRefreshToken.Value);
    }

    [InlineData("")]
    [InlineData(" ")]
    [InlineData("My refresh token")]
    [Theory]
    public async Task TestRetrieveAsync_Valid_NotContains(String val)
    {
        // Arrange
        this._dataSource._userRefreshTokensFunc = () =>
        {
            IQueryable<UserRefreshTokenEntity> query =
                Array
                    .Empty<UserRefreshTokenEntity>()
                    .AsQueryable();

            return new MockDataSet<UserRefreshTokenEntity>(query);
        };

        // Act
        UserRefreshTokenEntity? userRefreshToken =
            await this._repository.RetrieveAsync(
                val,
                TestContext.Current.CancellationToken
            );

        // Assert
        Assert.Null(userRefreshToken);
    }
#endregion
}
