using System;

namespace Shipstone.Authenticator.Api.Core;

/// <summary>
/// Represents a user.
/// </summary>
public interface IUser
{
    /// <summary>
    /// Gets the date the <see cref="IUser" /> was born.
    /// </summary>
    /// <value>The date the <see cref="IUser" /> was born.</value>
    DateOnly Born { get; }

    /// <summary>
    /// Gets the date and time the <see cref="IUser" /> last consented to policy.
    /// </summary>
    /// <value>The date and time the <see cref="IUser" /> last consented to policy.</value>
    DateTime Consented { get; }

    /// <summary>
    /// Gets the date and time the <see cref="IUser" /> was created.
    /// </summary>
    /// <value>The date and time the <see cref="IUser" /> was created.</value>
    DateTime Created { get; }

    /// <summary>
    /// Gets the email address of the <see cref="IUser" />.
    /// </summary>
    /// <value>The email address of the <see cref="IUser" />.</value>
    String EmailAddress { get; }

    /// <summary>
    /// Gets the forename of the <see cref="IUser" />.
    /// </summary>
    /// <value>The forename of the <see cref="IUser" />.</value>
    String Forename { get; }

    /// <summary>
    /// Gets the ID of the <see cref="IUser" />.
    /// </summary>
    /// <value>A <see cref="Guid" /> containing the ID of the <see cref="IUser" />.</value>
    Guid Id { get; }

    /// <summary>
    /// Gets the surname of the <see cref="IUser" />.
    /// </summary>
    /// <value>The surname of the <see cref="IUser" />.</value>
    String Surname { get; }

    /// <summary>
    /// Gets the date and time the <see cref="IUser" /> was last updated.
    /// </summary>
    /// <value>The date and time the <see cref="IUser" /> was last updated.</value>
    DateTime Updated { get; }
}
