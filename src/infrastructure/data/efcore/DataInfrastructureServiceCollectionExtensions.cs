using System;
using Microsoft.Extensions.DependencyInjection;

using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;

namespace Shipstone.Authenticator.Api.Infrastructure.Data;

/// <summary>
/// Provides a set of <c>static</c> (<c>Shared</c> in Visual Basic) methods for registering services with objects that implement <see cref="IServiceCollection" />.
/// </summary>
public static class DataInfrastructureServiceCollectionExtensions
{
    /// <summary>
    /// Registers Authenticator data infrastructure services with the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to register services with.</param>
    /// <returns>A reference to <c><paramref name="services" /></c> that can be further used to register services.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="services" /></c> is <c>null</c>.</exception>
    public static IServiceCollection AddAuthenticatorInfrastructureData(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddScoped<IRepository, Repository>()
            .AddScoped<IUserRefreshTokenRepository, UserRefreshTokenRepository>()
            .AddScoped<IUserRepository, UserRepository>();
    }
}
