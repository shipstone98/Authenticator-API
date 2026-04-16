using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Authenticator.Api.Core;
using Shipstone.Authenticator.Api.Core.Accounts;
using Shipstone.Authenticator.Api.Core.Passwords;
using Shipstone.Authenticator.Api.Core.Users;

using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.CoreTest;

public sealed class CoreServiceCollectionExtensionsTest
{
    [Fact]
    public void TestAddAuthenticatorCore_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                CoreServiceCollectionExtensions.AddAuthenticatorCore(null!));

        // Assert
        Assert.Equal("services", ex.ParamName);
    }

    [Fact]
    public void TestAddAuthenticatorCore_Valid()
    {
        // Arrange
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;

        // Act
        IServiceCollection result =
            CoreServiceCollectionExtensions.AddAuthenticatorCore(services);

        // Assert
        Assert.Same(services, result);

        IEnumerable<Type> types = new Type[]
        {
            typeof (IAuthenticateHandler),
            typeof (IOtpAuthenticateHandler),
            typeof (IPasswordResetHandler),
            typeof (IPasswordSetHandler),
            typeof (IPasswordUpdateHandler),
            typeof (IRefreshAuthenticateHandler),
            typeof (IRegisterHandler),
            typeof (IUnregisterHandler),
            typeof (IUserUpdateHandler)
        };

        foreach (Type type in types)
        {
            ServiceDescriptor descriptor =
                collection.First(s => s.ServiceType.Equals(type));

            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        }
    }
}
