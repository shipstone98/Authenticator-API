using System;
using Microsoft.AspNetCore.Builder;

using Shipstone.Authenticator.Api.Core;
using Shipstone.Authenticator.Api.Web.Middleware;

namespace Shipstone.Authenticator.Api.Web;

/// <summary>
/// Provides a set of <c>static</c> (<c>Shared</c> in Visual Basic) methods for adding middleware to objects that implement <see cref="IApplicationBuilder" />.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds Authenticator web claims middleware to the specified <see cref="IApplicationBuilder" />.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder" /> to add middleware to with.</param>
    /// <returns>A reference to <c><paramref name="app" /></c> that can be further used to add middleware.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="app" /></c> is <c>null</c>.</exception>
    public static IApplicationBuilder UseAuthenticatorWebClaims(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<ClaimsMiddleware>();
    }

    /// <summary>
    /// Adds Authenticator web <see cref="ConflictException" /> middleware to the specified <see cref="IApplicationBuilder" />.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder" /> to add middleware to with.</param>
    /// <returns>A reference to <c><paramref name="app" /></c> that can be further used to add middleware.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="app" /></c> is <c>null</c>.</exception>
    public static IApplicationBuilder UseAuthenticatorWebConflictExceptionHandling(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<ConflictExceptionHandlingMiddleware>();
    }
}
