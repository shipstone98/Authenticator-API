using System;
using Xunit;

using Shipstone.Authenticator.Api.Core;

namespace Shipstone.Authenticator.Api.CoreTest;

internal static class Internals
{
    internal static void AssertEqual(
        this IUser user,
        Guid id,
        DateTime created,
        DateTime updated,
        String emailAddress,
        String forename,
        String surname,
        DateOnly born,
        DateTime consented
    )
    {
        Assert.Equal(born, user.Born);
        Assert.Equal(consented, user.Consented);
        Assert.Equal(DateTimeKind.Utc, user.Consented.Kind);
        Assert.Equal(created, user.Created);
        Assert.Equal(DateTimeKind.Utc, user.Created.Kind);
        Assert.Equal(emailAddress, user.EmailAddress);
        Assert.Equal(forename, user.Forename);
        Assert.Equal(id, user.Id);
        Assert.Equal(surname, user.Surname);
        Assert.Equal(updated, user.Updated);
    }

    internal static void SetId(this Object entity, Object id)
    {
        Object?[]? arguments = new Object?[1] { id };

        entity
            .GetType()
            .GetProperty("Id")!
            .GetSetMethod()!
            .Invoke(entity, arguments);
    }
}
