using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Extensions.Security;

using Shipstone.Authenticator.Api.Core;
using Shipstone.Authenticator.Api.Core.Accounts;
using Shipstone.Authenticator.Api.Infrastructure.Authentication;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;
using Shipstone.Authenticator.Api.Infrastructure.Mail;

using Shipstone.Authenticator.Api.CoreTest.Mocks;
using Shipstone.Authenticator.Api.Test.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.CoreTest.Accounts;

public sealed class RegisterHandlerTest
{
    private readonly MockAuthenticationService _authentication;
    private readonly IRegisterHandler _handler;
    private readonly MockMailService _mail;
    private readonly MockNormalizationService _normalization;
    private readonly MockRepository _repository;

    public RegisterHandlerTest()
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
        MockNormalizationService normalization = new();
        services.AddSingleton<INormalizationService>(normalization);
        MockRepository repository = new();
        services.AddSingleton<IRepository>(repository);
        IServiceProvider provider = new MockServiceProvider(services);
        this._authentication = authentication;
        this._handler = provider.GetRequiredService<IRegisterHandler>();
        this._mail = mail;
        this._normalization = normalization;
        this._repository = repository;
    }

#region HandleAsync method
#region Invalid arguments
    [Fact]
    public async Task TestHandleAsync_Invalid_BornInvalid_InvalidDate()
    {
        // Arrange
        DateOnly born =
            DateOnly
                .FromDateTime(DateTime.UtcNow.Date)
                .AddDays(1);

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentException>(() =>
                this._handler.HandleAsync(
                    "john.doe@contoso.com",
                    "John",
                    "Doe",
                    born,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("born", ex.ParamName);
    }

    [Fact]
    public async Task TestHandleAsync_Invalid_BornInvalid_ValidDate()
    {
        // Arrange
        DateOnly born =
            DateOnly
                .FromDateTime(DateTime.UtcNow.Date)
                .AddYears(-18)
                .AddDays(1);

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentException>(() =>
                this._handler.HandleAsync(
                    "john.doe@contoso.com",
                    "John",
                    "Doe",
                    born,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("born", ex.ParamName);
    }

    [Fact]
    public Task TestHandleAsync_Invalid_EmailAddressInvalid() => throw new NotImplementedException();

    [Fact]
    public async Task TestHandleAsync_Invalid_EmailAddressNull()
    {
        // Arrange
        DateOnly born =
            DateOnly
                .FromDateTime(DateTime.UtcNow.Date)
                .AddYears(-18);

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    null!,
                    "John",
                    "Doe",
                    born,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("emailAddress", ex.ParamName);
    }

    [Fact]
    public Task TestHandleAsync_Invalid_ForenameInvalid() => throw new NotImplementedException();

    [Fact]
    public async Task TestHandleAsync_Invalid_ForenameNull()
    {
        // Arrange
        DateOnly born =
            DateOnly
                .FromDateTime(DateTime.UtcNow.Date)
                .AddYears(-18);

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    "john.doe@contoso.com",
                    null!,
                    "Doe",
                    born,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("forename", ex.ParamName);
    }

    [Fact]
    public Task TestHandleAsync_Invalid_SurnameInvalid() => throw new NotImplementedException();

    [Fact]
    public async Task TestHandleAsync_Invalid_SurnameNull()
    {
        // Arrange
        DateOnly born =
            DateOnly
                .FromDateTime(DateTime.UtcNow.Date)
                .AddYears(-18);

        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._handler.HandleAsync(
                    "john.doe@contoso.com",
                    "John",
                    null!,
                    born,
                    CancellationToken.None
                ));

        // Assert
        Assert.Equal("surname", ex.ParamName);
    }
#endregion

#region Valid arguments
    [InlineData(false, null)]
    [InlineData(true, "My password hash")]
    [Theory]
    public async Task TestHandleAsync_Valid_Failure(
        bool isActive,
        String? passwordHash
    )
    {
        // Arrange
        DateOnly born =
            DateOnly
                .FromDateTime(DateTime.UtcNow.Date)
                .AddYears(-18);

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = _ =>
                new UserEntity
                {
                    IsActive = isActive,
                    PasswordHash = passwordHash
                };

            return users;
        };

        // Act and assert
        await Assert.ThrowsAsync<ConflictException>(() =>
            this._handler.HandleAsync(
                "john.doe@contoso.com",
                "John",
                "Doe",
                born,
                CancellationToken.None
            ));
    }

    [Fact]
    public async Task TestHandleAsync_Valid_Success_Exists()
    {
#region Arrange
        // Arrange
        Guid id = Guid.NewGuid();
        DateTime created = DateTime.UnixEpoch.ToUniversalTime();
        const String EMAIL_ADDRESS = " john.doe@contoso.com ";
        const String FORENAME = " John ";
        const String SURNAME = " Doe ";

        DateOnly born =
            DateOnly
                .FromDateTime(DateTime.UtcNow.Date)
                .AddYears(-18);

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();

            users._retrieve_StringFunc = ea =>
                new UserEntity
                {
                    Born = born,
                    Created = created,
                    EmailAddress = ea,
                    Id = id,
                    IsActive = true
                };

            users._updateAction = _ => { };
            return users;
        };

        this._normalization._normalizeFunc = _ => String.Empty;

        this._authentication._generateOtpAction = (u, _) =>
            u.OtpExpires = DateTime.MaxValue;

        this._repository._saveAction = () => { };
        this._mail._sendRegistrationAction = (_, _) => { };
        DateTime notBefore = DateTime.UtcNow;
#endregion

        // Act
        IUser user =
            await this._handler.HandleAsync(
                EMAIL_ADDRESS,
                FORENAME,
                SURNAME,
                born,
                CancellationToken.None
            );

        // Assert
        Assert.False(DateTime.Compare(notBefore, user.Updated) > 0);

        user.AssertEqual(
            id,
            created,
            user.Updated,
            EMAIL_ADDRESS.Trim(),
            FORENAME.Trim(),
            SURNAME.Trim(),
            born,
            user.Updated
        );
    }

    [Fact]
    public async Task TestHandleAsync_Valid_Success_NotExists()
    {
#region Arrange
        // Arrange
        Guid id = Guid.NewGuid();
        const String EMAIL_ADDRESS = " john.doe@contoso.com ";
        const String FORENAME = " John ";
        const String SURNAME = " Doe ";

        DateOnly born =
            DateOnly
                .FromDateTime(DateTime.UtcNow.Date)
                .AddYears(-18);

        this._repository._usersFunc = () =>
        {
            MockUserRepository users = new();
            users._retrieve_StringFunc = _ => null;
            users._createAction = u => u.SetId(id);
            return users;
        };

        this._normalization._normalizeFunc = _ => String.Empty;

        this._authentication._generateOtpAction = (u, _) =>
            u.OtpExpires = DateTime.MaxValue;

        this._repository._saveAction = () => { };
        this._mail._sendRegistrationAction = (_, _) => { };
        DateTime notBefore = DateTime.UtcNow;
#endregion

        // Act
        IUser user =
            await this._handler.HandleAsync(
                EMAIL_ADDRESS,
                FORENAME,
                SURNAME,
                born,
                CancellationToken.None
            );

        // Assert
        Assert.False(DateTime.Compare(notBefore, user.Created) > 0);

        user.AssertEqual(
            id,
            user.Created,
            user.Created,
            EMAIL_ADDRESS.Trim(),
            FORENAME.Trim(),
            SURNAME.Trim(),
            born,
            user.Created
        );
    }
#endregion
#endregion
}
