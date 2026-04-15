using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Authenticator.Api.Core;
using Shipstone.Authenticator.Api.Core.Passwords;
using Shipstone.Authenticator.Api.Infrastructure.Authentication;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;
using Shipstone.Authenticator.Api.Infrastructure.Mail;

using Shipstone.Authenticator.Api.CoreTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.CoreTest.Passwords;

public sealed class PasswordResetHandlerTest
{
    private readonly MockAuthenticationService _authentication;
    private readonly IPasswordResetHandler _handler;
    private readonly MockMailService _mail;
    private readonly MockRepository _repository;

    public PasswordResetHandlerTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddAuthenticatorCore();
        MockAuthenticationService authentication = new();
        services.AddSingleton<IAuthenticationService>(authentication);
        MockMailService mail = new();
        services.AddSingleton<IMailService>(mail);
        MockRepository repository = new();
        services.AddSingleton<IRepository>(repository);
        IServiceProvider provider = new MockServiceProvider(services);
        this._authentication = authentication;
        this._handler = provider.GetRequiredService<IPasswordResetHandler>();
        this._mail = mail;
        this._repository = repository;
    }

#region HandleAsync method
    [Fact]
    public async Task TestHandleAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(null!, CancellationToken.None));

        // Assert
        Assert.Equal("emailAddress", ex.ParamName);
    }

#region Valid arguments
    [Fact]
    public Task TestHandleAsync_Valid_EmailAddressNotFound()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_StringFunc = _ => null;
            return users;
        };

        // Act
        return this._handler.HandleAsync(String.Empty, CancellationToken.None);

        // Nothing to assert
    }

    [Fact]
    public Task TestHandleAsync_Valid_EmailAddressFound_UserActive()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = _ =>
                new UserEntity
                {
                    IsActive = true
                };

            users._updateAction = _ => { };
            return users;
        };

        this._authentication._generateOtpAction = (u, _) =>
            u.OtpExpires = DateTime.MaxValue;

        this._repository._saveAction = () => { };
        this._mail._sendPasswordResetAction = (_, _) => { };

        // Act
        return this._handler.HandleAsync(String.Empty, CancellationToken.None);

        // Nothing to assert
    }

    [Fact]
    public Task TestHandleAsync_Valid_EmailAddressFound_UserNotActive()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_StringFunc = _ => new();
            return users;
        };

        // Act
        return this._handler.HandleAsync(String.Empty, CancellationToken.None);

        // Nothing to assert
    }
#endregion
#endregion
}
