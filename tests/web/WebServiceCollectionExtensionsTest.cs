using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Authenticator.Api.Core;
using Shipstone.Authenticator.Api.Web;

using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.WebTest;

public sealed class WebServiceCollectionExtensionsTest
{
    [Fact]
    public void TestAddAuthenticatorWebClaims_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                WebServiceCollectionExtensions.AddAuthenticatorWebClaims(null!));

        // Assert
        Assert.Equal("services", ex.ParamName);
    }

    [Fact]
    public void TestAddAuthenticatorWebClaims_Valid()
    {
        // Arrange
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;

        // Act
        IServiceCollection result =
            WebServiceCollectionExtensions.AddAuthenticatorWebClaims(services);

        // Assert
        Assert.Same(services, result);

        ServiceDescriptor serviceDescriptor =
            collection.First(s =>
                s.ServiceType.Equals(typeof (IClaimsService)));

        Assert.Equal(ServiceLifetime.Scoped, serviceDescriptor.Lifetime);

        ServiceDescriptor middlewareDescriptor =
            collection.First(s =>
                s.ServiceType.IsAssignableTo(typeof (IMiddleware)));

        Assert.Equal(ServiceLifetime.Scoped, middlewareDescriptor.Lifetime);
    }
}
