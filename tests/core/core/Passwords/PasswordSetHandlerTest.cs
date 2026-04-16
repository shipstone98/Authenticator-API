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
using Shipstone.Authenticator.Api.Core.Passwords;
using Shipstone.Authenticator.Api.Infrastructure.Authentication;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;
using Shipstone.Authenticator.Api.Infrastructure.Mail;

using Shipstone.Authenticator.Api.CoreTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.CoreTest.Passwords;

public sealed class PasswordSetHandlerTest
{
    private readonly IPasswordSetHandler _handler;
    private readonly MockPasswordService _password;
    private readonly MockRepository _repository;

    public PasswordSetHandlerTest()
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
        this._handler = provider.GetRequiredService<IPasswordSetHandler>();
        this._password = password;
        this._repository = repository;
    }

#region HandleAsync method
#region Invalid arguments
    [Fact]
    public async Task TestHandleAsync_Invalid_EmailAddressNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    null!,
                    String.Empty,
                    String.Empty,
                    TestContext.Current.CancellationToken
                ));

        // Assert
        Assert.Equal("emailAddress", ex.ParamName);
    }

    [Fact]
    public async Task TestHandleAsync_Invalid_OtpNull()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    String.Empty,
                    null!,
                    String.Empty,
                    TestContext.Current.CancellationToken
                ));

        // Assert
        Assert.Equal("otp", ex.ParamName);
    }

    [Fact]
    public async Task TestHandleAsync_Invalid_PasswordInvalid()
    {
        // Arrange
        ArgumentException innerException = new(null as String, "password");
        this._password._validateAction = _ => throw innerException;

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentException>(() =>
                this._handler.HandleAsync(
                    String.Empty,
                    String.Empty,
                    null!,
                    TestContext.Current.CancellationToken
                ));

        // Assert
        Assert.Same(innerException, ex);
        Assert.Equal("password", ex.ParamName);
    }
#endregion

#region Valid arguments
#region Failure
    [Fact]
    public Task TestHandleAsync_Valid_Failure_EmailAddressNotFound()
    {
        // Arrange
        this._password._validateAction = _ => { };

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
                String.Empty,
                TestContext.Current.CancellationToken
            ));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_OtpExpired_OtpExpiresNotNull()
    {
        // Arrange
        const String OTP = "123456";
        this._password._validateAction = _ => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = _ =>
                new UserEntity
                {
                    IsActive = true,
                    Otp = OTP,
                    OtpExpires = DateTime.MinValue
                };

            users._updateAction = _ => { };
            return users;
        };

        this._repository._saveAction = () => { };

        // Act and assert
        return Assert.ThrowsAsync<ForbiddenException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                OTP,
                String.Empty,
                TestContext.Current.CancellationToken
            ));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_OtpExpired_OtpExpiresNull()
    {
        // Arrange
        const String OTP = "123456";
        this._password._validateAction = _ => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = _ =>
                new UserEntity
                {
                    IsActive = true,
                    Otp = OTP
                };

            users._updateAction = _ => { };
            return users;
        };

        this._repository._saveAction = () => { };

        // Act and assert
        return Assert.ThrowsAsync<ForbiddenException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                OTP,
                String.Empty,
                TestContext.Current.CancellationToken
            ));
    }

    [InlineData(null)]
    [InlineData("123456")]
    [Theory]
    public Task TestHandleAsync_Valid_Failure_OtpNotEqual(String? otp)
    {
        // Arrange
        this._password._validateAction = _ => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = _ =>
                new UserEntity
                {
                    IsActive = true,
                    Otp = otp
                };

            return users;
        };

        // Act and assert
        return Assert.ThrowsAsync<ForbiddenException>(() =>
            this._handler.HandleAsync(
                String.Empty,
                String.Empty,
                String.Empty,
                TestContext.Current.CancellationToken
            ));
    }

    [Fact]
    public Task TestHandleAsync_Valid_Failure_UserNotActive()
    {
        // Arrange
        this._password._validateAction = _ => { };

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
                String.Empty,
                TestContext.Current.CancellationToken
            ));
    }
#endregion

    [Fact]
    public async Task TestHandleAsync_Valid_Success()
    {
        // Arrange
        const String OTP = "123456";
        this._password._validateAction = _ => { };

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = _ =>
                new UserEntity
                {
                    IsActive = true,
                    Otp = OTP,
                    OtpExpires = DateTime.MaxValue
                };

            users._updateAction = _ => { };
            return users;
        };

        this._repository._saveAction = () => { };
        this._password._hashFunc = _ => String.Empty;

        // Act and assert
        await this._handler.HandleAsync(
            String.Empty,
            OTP,
            String.Empty,
            TestContext.Current.CancellationToken
        );
    }
#endregion
#endregion
}
