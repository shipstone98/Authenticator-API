using System;
using Microsoft.Extensions.DependencyInjection;

namespace Shipstone.Authenticator.Api.Infrastructure.Authentication;

/// <summary>
/// Provides a set of <c>static</c> (<c>Shared</c> in Visual Basic) methods for registering services with objects that implement <see cref="IServiceCollection" />.
/// </summary>
public static class AuthenticationInfrastructureServiceCollectionExtensions
{
    /// <summary>
    /// Registers Authenticator authentication infrastructure services with the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to register services with.</param>
    /// <param name="configureAuthentication">A delegate to configure <see cref="AuthenticationOptions" />, or <c>null</c>.</param>
    /// <returns>A reference to <c><paramref name="services" /></c> that can be further used to register services.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="services" /></c> is <c>null</c>.</exception>
    public static IServiceCollection AddAuthenticatorInfrastructureAuthentication(
        this IServiceCollection services,
        Action<AuthenticationOptions>? configureAuthentication = null
    )
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddSingleton<IAuthenticationService, AuthenticationService>()
            .Configure<AuthenticationOptions>(options =>
            {
                if (configureAuthentication is not null)
                {
                    configureAuthentication(options);
                }
            });
    }
}
