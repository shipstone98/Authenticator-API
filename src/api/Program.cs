using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Shipstone.AspNetCore.Http;
using Shipstone.Extensions.Identity;
using Shipstone.Extensions.Security;
using Shipstone.Utilities.Security.Cryptography;

using Shipstone.Authenticator.Api.Core;
using Shipstone.Authenticator.Api.Infrastructure.Authentication;
using Shipstone.Authenticator.Api.Infrastructure.Data;
using Shipstone.Authenticator.Api.Infrastructure.Data.MySql;
using Shipstone.Authenticator.Api.Infrastructure.Mail;
using Shipstone.Authenticator.Api.Web;
using Shipstone.Authenticator.Api.WebApi;

WebApplicationBuilder builder = WebApplication.CreateBuilder();

IConfigurationSection authenticationSection =
    builder.Configuration.GetRequiredSection("Authentication");

String? connectionString =
    builder.Configuration.GetConnectionString("MySql");

bool isNcsaCommonLoggingEnabled =
    builder.Configuration.GetValue<bool>("IsNcsaCommonLoggingEnabled");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        String? signingKey = authenticationSection["AccessTokenSigningKey"];

        if (signingKey is null)
        {
            throw new InvalidOperationException("The provided configuration does not contain a valid access token signing key.");
        }

        byte[] bytes = Convert.FromBase64String(signingKey);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(bytes),
            ValidAudience = authenticationSection["Audience"],
            ValidIssuer = authenticationSection["Issuer"],
            ValidateIssuerSigningKey = true
        };
    });

builder.Services
    .AddControllers()
    .AddAuthenticatorControllers();

builder.Services
    .AddIdentityExtensions()
    .AddSecurityExtensions(
        builder.Configuration
            .GetRequiredSection("Security")
            .Bind
    )
    .AddAuthenticatorCore()
    .AddAuthenticatorInfrastructureAuthentication(authenticationSection.Bind)
    .AddAuthenticatorInfrastructureData()
    .AddAuthenticatorInfrastructureDataMySql(connectionString)
    .AddAuthenticatorWebClaims()
    .AddAuthenticatorWebConflictExceptionHandling()
    .AddSingleton<IEncryptionService, StubEncryptionService>()
    .AddSingleton<IMailService, StubMailService>()
    .AddSingleton<RandomNumberGenerator>(_ =>
    {
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        return new ConcurrentRandomNumberGenerator(rng);
    })
    .AddSingleton<JwtSecurityTokenHandler>()
    .AddSingleton<IPasswordHasher<IPasswordService>, PasswordHasher<IPasswordService>>();

if (isNcsaCommonLoggingEnabled)
{
    TextWriter writer = new StreamWriter("log.txt", true);
    builder.Services.AddNcsaCommonLogging(writer);
}

WebApplication app = builder.Build();

if (isNcsaCommonLoggingEnabled)
{
    app.UseNcsaCommonLogging();
}

app.UseAuthenticatorWebConflictExceptionHandling();
app.UseAuthentication();
app.UseAuthenticatorWebClaims();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();
return 0;
