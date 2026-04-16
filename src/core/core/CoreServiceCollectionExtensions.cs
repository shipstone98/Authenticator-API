using System;
using Microsoft.Extensions.DependencyInjection;

using Shipstone.Authenticator.Api.Core.Accounts;
using Shipstone.Authenticator.Api.Core.Passwords;
using Shipstone.Authenticator.Api.Core.Services;
using Shipstone.Authenticator.Api.Core.Users;

namespace Shipstone.Authenticator.Api.Core;

/// <summary>
/// Provides a set of <c>static</c> (<c>Shared</c> in Visual Basic) methods for registering services with objects that implement <see cref="IServiceCollection" />.
/// </summary>
public static class CoreServiceCollectionExtensions
{
    /// <summary>
    /// Registers Authenticator core services with the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to register services with.</param>
    /// <returns>A reference to <c><paramref name="services" /></c> that can be further used to register services.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="services" /></c> is <c>null</c>.</exception>
    public static IServiceCollection AddAuthenticatorCore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddSingleton<IValidationService, ValidationService>()
            .AddScoped<IAuthenticateHandler, AuthenticateHandler>()
            .AddScoped<IAuthenticateService, AuthenticateService>()
            .AddScoped<IOtpAuthenticateHandler, OtpAuthenticateHandler>()
            .AddScoped<IOtpService, OtpService>()
            .AddScoped<IPasswordResetHandler, PasswordResetHandler>()
            .AddScoped<IPasswordSetHandler, PasswordSetHandler>()
            .AddScoped<IPasswordUpdateHandler, PasswordUpdateHandler>()
            .AddScoped<IRefreshAuthenticateHandler, RefreshAuthenticateHandler>()
            .AddScoped<IRegisterHandler, RegisterHandler>()
            .AddScoped<IUnregisterHandler, UnregisterHandler>()
            .AddScoped<IUserUpdateHandler, UserUpdateHandler>();
    }
}
