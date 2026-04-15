using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

using Shipstone.Extensions.Security;

using Shipstone.Authenticator.Api.Infrastructure.Data;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;

using Shipstone.Authenticator.Api.Infrastructure.DataTest.Mocks;
using Shipstone.Authenticator.Api.Test.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.Infrastructure.DataTest.Repositories;

public sealed class RepositoryTest
{
    private readonly MockDataSource _dataSource;
    private readonly IRepository _repository;

    public RepositoryTest()
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
        this._repository = provider.GetRequiredService<IRepository>();
    }

    [Fact]
    public void TestUserRefreshTokens_Get()
    {
        // Act and assert
        Assert.NotNull(this._repository.UserRefreshTokens);
    }

    [Fact]
    public void TestUsers_Get()
    {
        // Act and assert
        Assert.NotNull(this._repository.Users);
    }

    [Fact]
    public Task TestSaveAsync()
    {
        // Arrange
        this._dataSource._saveAction = () => { };

        // Act
        return this._repository.SaveAsync(CancellationToken.None);

        // Nothing to assert
    }
}
