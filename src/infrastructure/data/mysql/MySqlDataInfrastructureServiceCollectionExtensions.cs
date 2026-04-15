using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Shipstone.Extensions.Security;

namespace Shipstone.Authenticator.Api.Infrastructure.Data.MySql;

/// <summary>
/// Provides a set of <c>static</c> (<c>Shared</c> in Visual Basic) methods for registering services with objects that implement <see cref="IServiceCollection" />.
/// </summary>
public static class MySqlDataInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddAuthenticatorInfrastructureDataMySql(
        this IServiceCollection services,
        String? connectionString = null
    )
    {
        ArgumentNullException.ThrowIfNull(services);

        DbContextOptionsBuilder<MySqlDbContext> optionsBuilder = new();

        ServerVersion serverVersion =
            ServerVersion.AutoDetect(connectionString);

        optionsBuilder.UseMySql(connectionString, serverVersion);

        return services
            .AddScoped<IDataSource>(provider =>
                provider.GetRequiredService<MySqlDbContext>())
            .AddScoped(provider =>
            {
                IEncryptionService encryption =
                    provider.GetRequiredService<IEncryptionService>();

                return new MySqlDbContext(optionsBuilder.Options, encryption);
            });
    }
}
