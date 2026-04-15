using System;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace Shipstone.Authenticator.Api.Web;

/// <summary>
/// Provides a set of <c>static</c> (<c>Shared</c> in Visual Basic) methods for adding controllers to objects that implement <see cref="IMvcBuilder" />.
/// </summary>
public static class MvcBuilderExtensions
{
    /// <summary>
    /// Adds Authenticator controllers to the specified <see cref="IMvcBuilder" />.
    /// </summary>
    /// <param name="builder">The <see cref="IServiceCollection" /> to add controllers to.</param>
    /// <returns>A reference to <c><paramref name="builder" /></c> that can be further used to add controllers.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="builder" /></c> is <c>null</c>.</exception>
    public static IMvcBuilder AddAuthenticatorControllers(this IMvcBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.ConfigureApplicationPartManager(manager =>
        {
            IApplicationFeatureProvider provider =
                new AuthenticatorControllerFeatureProvider();

            manager.FeatureProviders.Add(provider);
        });
    }
}
