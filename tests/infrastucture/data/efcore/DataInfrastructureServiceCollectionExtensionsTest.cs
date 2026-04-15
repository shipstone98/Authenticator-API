using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Authenticator.Api.Infrastructure.Data;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;

using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.Infrastructure.DataTest;

public sealed class DataInfrastructureServiceCollectionExtensionsTest
{
    [Fact]
    public void TestAddAuthenticatorInfrastructureData_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                DataInfrastructureServiceCollectionExtensions.AddAuthenticatorInfrastructureData(null!));

        // Assert
        Assert.Equal("services", ex.ParamName);
    }

    [Fact]
    public void TestAddAuthenticatorInfrastructureData_Valid()
    {
        // Arrange
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;

        // Act
        IServiceCollection result =
            DataInfrastructureServiceCollectionExtensions.AddAuthenticatorInfrastructureData(services);

        // Assert
        Assert.Same(services, result);

        IEnumerable<Type> serviceTypes = new Type[]
        {
            typeof (IRepository),
            typeof (IUserRefreshTokenRepository),
            typeof (IUserRepository)
        };

        foreach (Type serviceType in serviceTypes)
        {
            ServiceDescriptor descriptor =
                collection.First(s => s.ServiceType.Equals(serviceType));

            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        }
    }
}
