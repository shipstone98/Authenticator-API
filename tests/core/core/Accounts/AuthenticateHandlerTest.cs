using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Extensions.Identity;
using Shipstone.Utilities;

using Shipstone.Authenticator.Api.Core;
using Shipstone.Authenticator.Api.Core.Accounts;
using Shipstone.Authenticator.Api.Infrastructure.Authentication;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;
using Shipstone.Authenticator.Api.Infrastructure.Mail;

using Shipstone.Authenticator.Api.CoreTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.CoreTest.Accounts;

public sealed class AuthenticateHandlerTest
{
    private readonly MockAuthenticationService _authentication;
    private readonly IAuthenticateHandler _handler;
    private readonly MockMailService _mail;
    private readonly MockPasswordService _password;
    private readonly MockRepository _repository;

    public AuthenticateHandlerTest()
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
        MockPasswordService password = new();
        services.AddSingleton<IPasswordService>(password);
        MockRepository repository = new();
        services.AddSingleton<IRepository>(repository);
        IServiceProvider provider = new MockServiceProvider(services);
        this._authentication = authentication;
        this._handler = provider.GetRequiredService<IAuthenticateHandler>();
        this._mail = mail;
        this._password = password;
        this._repository = repository;
    }

#region HandleAsync method
    [Fact]
    public async Task TestHandleAsync_Invalid_EmailAddressNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    null!,
                    String.Empty,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("emailAddress", ex.ParamName);
    }

    [Fact]
    public async Task TestHandleAsync_Invalid_PasswordNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    String.Empty,
                    null!,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("password", ex.ParamName);
    }

#region Valid arguments
#region Failure
    [Fact]
    public Task TestHandleAsync_Valid_Failure_EmailAddressNotFound()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_StringFunc = _ => null;
            return users;
        };

        // Act and assert
        return Assert.ThrowsAsync<NotFoundException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                String.Empty,
                CancellationToken.None
            ));
    }

    [Fact]
    public async Task TestHandleAsync_Valid_Failure_PasswordNotCorrect()
    {
        // Arrange
        Exception innerException = new IncorrectPasswordException();

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = _ =>
                new UserEntity
                {
                    IsActive = true,
                    PasswordHash = String.Empty
                };

            return users;
        };

        this._password._verifyFunc = (_, _) => throw innerException;

        // Act
        Exception ex =
            await Assert.ThrowsAsync<IncorrectPasswordException>(() =>
                this._handler.HandleAsync(
                    String.Empty,
                    String.Empty,
                    CancellationToken.None
                ));

        // Assert
        Assert.Same(innerException, ex.InnerException);
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotActive()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_StringFunc = _ => new();
            return users;
        };

        // Act and assert
        return Assert.ThrowsAsync<UserNotActiveException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                String.Empty,
                CancellationToken.None
            ));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotVerified()
    {
        // Arrange
        Exception innerException = new IncorrectPasswordException();

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = _ =>
                new UserEntity
                {
                    IsActive = true
                };

            return users;
        };

        // Act and assert
        return Assert.ThrowsAsync<ForbiddenException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                String.Empty,
                CancellationToken.None
            ));
    }
#endregion

    [Fact]
    public Task TestHandleAsync_Valid_Success_PasswordNotSecure()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = _ =>
                new UserEntity
                {
                    IsActive = true,
                    PasswordHash = String.Empty
                };

            users._updateAction = _ => { };
            return users;
        };

        this._password._verifyFunc = (_, _) => false;
        this._password._hashFunc = _ => String.Empty;

        this._authentication._generateOtpAction = (u, n) =>
            u.OtpExpires = n.AddDays(1);

        this._repository._saveAction = () => { };
        this._mail._sendOtpAction = (_, _) => { };

        // Act
        return this._handler.HandleAsync(
            String.Empty,
            String.Empty,
            CancellationToken.None
        );

        // Nothing to assert
    }

    [Fact]
    public Task TestHandleAsync_Valid_Success_PasswordSecure()
    {
        // Arrange
        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = _ =>
                new UserEntity
                {
                    IsActive = true,
                    PasswordHash = String.Empty
                };

            users._updateAction = _ => { };
            return users;
        };

        this._password._verifyFunc = (_, _) => true;

        this._authentication._generateOtpAction = (u, n) =>
            u.OtpExpires = n.AddDays(1);

        this._repository._saveAction = () => { };
        this._mail._sendOtpAction = (_, _) => { };

        // Act
        return this._handler.HandleAsync(
            String.Empty,
            String.Empty,
            CancellationToken.None
        );

        // Nothing to assert
    }
#endregion
#endregion
}
