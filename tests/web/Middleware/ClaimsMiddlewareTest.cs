using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Authenticator.Api.Core;
using Shipstone.Authenticator.Api.Web;

using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.WebTest.Middleware;

public sealed class ClaimsMiddlewareTest
{
    private readonly MockHttpContext _context;
    private readonly IClaimsService _claims;
    private readonly RequestDelegate _next;

    public ClaimsMiddlewareTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddAuthenticatorWebClaims();
        MockMiddlewareFactory middlewareFactory = new();
        services.AddSingleton<IMiddlewareFactory>(middlewareFactory);
        IServiceProvider provider = new MockServiceProvider(services);
        LinkedList<Func<RequestDelegate, RequestDelegate>> middleware = new();
        MockApplicationBuilder app = new();

        app._useFunc = m =>
        {
            middleware.AddFirst(m);
            return app;
        };

        app.UseAuthenticatorWebClaims();
        RequestDelegate next = _ => Task.CompletedTask;

        foreach (Func<RequestDelegate, RequestDelegate> item in middleware)
        {
            next = item(next);
        }

        MockHttpContext context = new();
        context._requestServicesFunc = () => provider;

        middlewareFactory._createFunc = t =>
            provider.GetService(t) as IMiddleware;

        middlewareFactory._releaseAction = _ => { };
        this._claims = provider.GetRequiredService<IClaimsService>();
        this._context = context;
        this._next = next;
    }

#region InvokeAsync method
    [Fact]
    public async Task TestInvokeAsync_ClaimsEmpty()
    {
        // Arrange
        this._context._userFunc = () => new();

        // Act
        await this._next.Invoke(this._context);

        // Assert
        this._claims.AssertNotAuthenticated();
    }

#region Claims not empty
#region Missing
    [Fact]
    public async Task TestInvokeAsync_ClaimsNotEmpty_Missing_EmailAddress()
    {
        // Arrange
        this._context._userFunc = () =>
        {
            String idString =
                Guid
                    .NewGuid()
                    .ToString();

            Claim idClaim = new(ClaimTypes.NameIdentifier, idString);
            Claim userNameClaim = new(ClaimTypes.Name, String.Empty);
            IEnumerable<Claim> claims = new Claim[] { idClaim, userNameClaim };
            IIdentity identity = new ClaimsIdentity(claims);
            return new(identity);
        };

        // Act
        await this._next.Invoke(this._context);

        // Assert
        this._claims.AssertNotAuthenticated();
    }

    [Fact]
    public async Task TestInvokeAsync_ClaimsNotEmpty_Missing_Id()
    {
        // Arrange
        this._context._userFunc = () =>
        {
            Claim emailAddressClaim = new(ClaimTypes.Name, String.Empty);
            Claim userNameClaim = new(ClaimTypes.Name, String.Empty);

            IEnumerable<Claim> claims =
                new Claim[] { emailAddressClaim, userNameClaim };

            IIdentity identity = new ClaimsIdentity(claims);
            return new(identity);
        };

        // Act
        await this._next.Invoke(this._context);

        // Assert
        this._claims.AssertNotAuthenticated();
    }

    [Fact]
    public async Task TestInvokeAsync_ClaimsNotEmpty_Missing_UserName()
    {
        // Arrange
        this._context._userFunc = () =>
        {
            String idString =
                Guid
                    .NewGuid()
                    .ToString();

            Claim emailAddressClaim = new(ClaimTypes.Name, String.Empty);
            Claim idClaim = new(ClaimTypes.NameIdentifier, idString);

            IEnumerable<Claim> claims =
                new Claim[] { emailAddressClaim, idClaim };

            IIdentity identity = new ClaimsIdentity(claims);
            return new(identity);
        };

        // Act
        await this._next.Invoke(this._context);

        // Assert
        this._claims.AssertNotAuthenticated();
    }
#endregion

    [Fact]
    public async Task TestInvokeAsync_ClaimsNotEmpty_NotMissing_Correct()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        const String EMAIL_ADDRESS = "john.doe@contoso.com";
        const String USER_NAME = "johndoe2025";

        this._context._userFunc = () =>
        {
            String idString = id.ToString();
            Claim emailAddressClaim = new(ClaimTypes.Email, EMAIL_ADDRESS);
            Claim idClaim = new(ClaimTypes.NameIdentifier, idString);
            Claim userNameClaim = new(ClaimTypes.Name, USER_NAME);

            IEnumerable<Claim> claims =
                new Claim[3] { emailAddressClaim, idClaim, userNameClaim };

            IIdentity identity = new ClaimsIdentity(claims);
            return new(identity);
        };

        // Act
        await this._next.Invoke(this._context);

        // Assert
        Assert.Equal(EMAIL_ADDRESS, this._claims.EmailAddress);
        Assert.Equal(id, this._claims.Id);
    }

    [Fact]
    public async Task TestInvokeAsync_ClaimsNotEmpty_NotMissing_NotCorrect()
    {
        // Arrange
        this._context._userFunc = () =>
        {
            Claim emailAddressClaim = new(ClaimTypes.Email, String.Empty);
            Claim idClaim = new(ClaimTypes.NameIdentifier, "12345");
            Claim userNameClaim = new(ClaimTypes.Name, String.Empty);

            IEnumerable<Claim> claims =
                new Claim[3] { emailAddressClaim, idClaim, userNameClaim };

            IIdentity identity = new ClaimsIdentity(claims);
            return new(identity);
        };

        // Act
        await this._next.Invoke(this._context);

        // Assert
        this._claims.AssertNotAuthenticated();
    }
#endregion
#endregion
}
