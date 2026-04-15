using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

using Shipstone.Authenticator.Api.Infrastructure.Authentication;

using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.Infrastructure.AuthenticationTest;

public sealed class AuthenticationInfrastructureServiceCollectionExtensionsTest
{
#region AddAuthenticatorInfrastructureAuthentication method
    [Fact]
    public void TestAddAuthenticatorAuthentication_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                AuthenticationInfrastructureServiceCollectionExtensions.AddAuthenticatorInfrastructureAuthentication(null!));

        // Assert
        Assert.Equal("services", ex.ParamName);
    }

    [Fact]
    public void TestAddAuthenticatorAuthentication_Valid()
    {
        // Arrange
        IList<ServiceDescriptor> collection = new List<ServiceDescriptor>();
        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._countFunc = () => collection.Count;
        services._itemFunc = i => collection[i];
        services._getEnumeratorFunc = collection.GetEnumerator;

        // Act
        IServiceCollection result =
            AuthenticationInfrastructureServiceCollectionExtensions.AddAuthenticatorInfrastructureAuthentication(services);

        // Assert
        Assert.Same(services, result);
        AuthenticationInfrastructureServiceCollectionExtensionsTest.AssertValid(services);
    }

    [Fact]
    public void TestAddAuthenticatorAuthentication_Valid_ConfigureAuthentication()
    {
        // Arrange
        const int OTP_EXPIRY_MINUTES = 123456789;
        IList<ServiceDescriptor> collection = new List<ServiceDescriptor>();
        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._countFunc = () => collection.Count;
        services._itemFunc = i => collection[i];
        services._getEnumeratorFunc = collection.GetEnumerator;

        // Act
        IServiceCollection result =
            AuthenticationInfrastructureServiceCollectionExtensions.AddAuthenticatorInfrastructureAuthentication(
                services,
                options => options.OtpExpiryMinutes = OTP_EXPIRY_MINUTES
            );

        // Assert
        Assert.Same(services, result);
        AuthenticationInfrastructureServiceCollectionExtensionsTest.AssertValid(services);
        IServiceProvider provider = new MockServiceProvider(services);

        IOptions<AuthenticationOptions> options =
            provider.GetRequiredService<IOptions<AuthenticationOptions>>();

        Assert.Equal(OTP_EXPIRY_MINUTES, options.Value.OtpExpiryMinutes);
    }
#endregion

    private static void AssertValid(IServiceCollection services)
    {
        ServiceDescriptor serviceDescriptor =
            services.First(s =>
                s.ServiceType.Equals(typeof (IAuthenticationService)));

        Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);

        ServiceDescriptor configureOptionsDescriptor =
            services.First(s =>
                s.ServiceType.Equals(typeof (IConfigureOptions<AuthenticationOptions>)));

        Assert.Equal(
            ServiceLifetime.Singleton,
            configureOptionsDescriptor.Lifetime
        );
    }
}
