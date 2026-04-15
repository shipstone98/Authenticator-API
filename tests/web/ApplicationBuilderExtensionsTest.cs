using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Xunit;

using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.WebTest;

public sealed class ApplicationBuilderExtensionsTest
{
    [Fact]
    public void TestUseAuthenticatorWebClaims_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                Web.ApplicationBuilderExtensions.UseAuthenticatorWebClaims(null!));

        // Assert
        Assert.Equal("app", ex.ParamName);
    }

    [Fact]
    public void TestUseAuthenticatorWebClaims_Valid()
    {
        // Arrange
        ICollection<Func<RequestDelegate, RequestDelegate>> middleware =
            new List<Func<RequestDelegate, RequestDelegate>>();

        MockApplicationBuilder app = new();

        app._useFunc = m =>
        {
            middleware.Add(m);
            return app;
        };

        // Act
        IApplicationBuilder result =
            Web.ApplicationBuilderExtensions.UseAuthenticatorWebClaims(app);

        // Assert
        Assert.Same(app, result);
        Assert.NotEmpty(middleware);
    }

    [Fact]
    public void TestUseAuthenticatorWebConflictExceptionHandling_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                Web.ApplicationBuilderExtensions.UseAuthenticatorWebConflictExceptionHandling(null!));

        // Assert
        Assert.Equal("app", ex.ParamName);
    }

    [Fact]
    public void TestUseAuthenticatorWebConflictExceptionHandling_Valid()
    {
        // Arrange
        ICollection<Func<RequestDelegate, RequestDelegate>> middleware =
            new List<Func<RequestDelegate, RequestDelegate>>();

        MockApplicationBuilder app = new();

        app._useFunc = m =>
        {
            middleware.Add(m);
            return app;
        };

        // Act
        IApplicationBuilder result =
            Web.ApplicationBuilderExtensions.UseAuthenticatorWebConflictExceptionHandling(app);

        // Assert
        Assert.Same(app, result);
        Assert.NotEmpty(middleware);
    }
}
