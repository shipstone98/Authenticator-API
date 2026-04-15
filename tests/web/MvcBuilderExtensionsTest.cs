using System;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Authenticator.Api.Web;

using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.WebTest;

public sealed class MvcBuilderExtensionsTest
{
    [Fact]
    public void TestAddAuthenticatorControllers_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                MvcBuilderExtensions.AddAuthenticatorControllers(null!));

        // Assert
        Assert.Equal("builder", ex.ParamName);
    }

    [Fact]
    public void TestAddAuthenticatorControllers_Valid()
    {
        // Arrange
        ApplicationPartManager manager = new();
        MockMvcBuilder builder = new();
        builder._partManagerFunc = () => manager;

        // Act
        IMvcBuilder result =
            MvcBuilderExtensions.AddAuthenticatorControllers(builder);

        // Assert
        Assert.Same(builder, result);
        
        Assert.Contains(
            manager.FeatureProviders,
            fp => fp is ControllerFeatureProvider
        );
    }
}
