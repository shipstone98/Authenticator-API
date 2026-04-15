using System;

using Shipstone.Extensions.Identity;
using Shipstone.Utilities;

using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Core;

internal static class PasswordServiceExtensions
{
    internal static bool Verify(
        this IPasswordService passwordService,
        UserEntity user,
        String password
    )
    {
        String? passwordHash = user.PasswordHash;

        if (passwordHash is null)
        {
            throw new ForbiddenException("The user whose email address matches the provided email address has not verified their email address.");
        }

        try
        {
            return passwordService.Verify(passwordHash, password);
        }

        catch (IncorrectPasswordException ex)
        {
            throw new IncorrectPasswordException(
                "The hashed representation of the provided password does not match the password hash of the user whose email address matches the provided email address.",
                ex
            );
        }
    }
}
