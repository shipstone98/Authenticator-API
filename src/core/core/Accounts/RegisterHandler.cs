using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Extensions.Security;
using Shipstone.Utilities;

using Shipstone.Authenticator.Api.Core.Services;
using Shipstone.Authenticator.Api.Infrastructure.Authentication;
using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;
using Shipstone.Authenticator.Api.Infrastructure.Mail;

namespace Shipstone.Authenticator.Api.Core.Accounts;

internal sealed class RegisterHandler : IRegisterHandler
{
    private readonly IAuthenticationService _authentication;
    private readonly IMailService _mail;
    private readonly INormalizationService _normalization;
    private readonly IRepository _repository;
    private readonly IValidationService _validation;

    public RegisterHandler(
        IRepository repository,
        IAuthenticationService authentication,
        IMailService mail,
        INormalizationService normalization,
        IValidationService validation
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(authentication);
        ArgumentNullException.ThrowIfNull(mail);
        ArgumentNullException.ThrowIfNull(normalization);
        ArgumentNullException.ThrowIfNull(validation);
        this._authentication = authentication;
        this._mail = mail;
        this._normalization = normalization;
        this._repository = repository;
        this._validation = validation;
    }

    private async Task<IUser> HandleAsync(
        String emailAddress,
        String forename,
        String surname,
        DateOnly born,
        DateTime now,
        CancellationToken cancellationToken
    )
    {
        bool isCreated;

        UserEntity? user =
            await this._repository.Users.RetrieveAsync(
                emailAddress,
                cancellationToken
            );

        if (user is null)
        {
            user = new UserEntity
            {
                Created = now,
                EmailAddress = emailAddress,
                EmailAddressNormalized =
                    this._normalization.Normalize(emailAddress),
                IsActive = true
            };

            isCreated = true;
        }

        else if (user.IsActive && user.PasswordHash is null)
        {
            isCreated = false;
        }

        else
        {
            throw new ConflictException("A user whose email address and/or name matches the provided email address and/or user name already exists.");
        }

        user.Born = born;
        user.Consented = now;
        user.Forename = forename;
        user.Surname = surname;
        user.Updated = now;

        await this._authentication.GenerateOtpAsync(
            user,
            now,
            cancellationToken
        );

        if (isCreated)
        {
            await this._repository.Users.CreateAsync(user, cancellationToken);
        }

        else
        {
            await this._repository.Users.UpdateAsync(user, cancellationToken);
        }

        await this._repository.SaveAsync(cancellationToken);
        TimeSpan difference = user.OtpExpires!.Value.Subtract(now);

        await this._mail.SendRegistrationAsync(
            user,
            (int) difference.TotalMinutes,
            cancellationToken
        );

        return new User(user);
    }

    Task<IUser> IRegisterHandler.HandleAsync(
        String emailAddress,
        String forename,
        String surname,
        DateOnly born,
        CancellationToken cancellationToken
    )
    {
        emailAddress = this._validation.ValidateEmailAddress(emailAddress);
        forename = this._validation.ValidateForename(forename);
        surname = this._validation.ValidateSurname(surname);
        DateTime now = DateTime.UtcNow;
        DateOnly today = DateOnly.FromDateTime(now);
        this._validation.ValidateBorn(born, today);

        return this.HandleAsync(
            emailAddress,
            forename,
            surname,
            born,
            now,
            cancellationToken
        );
    }
}
