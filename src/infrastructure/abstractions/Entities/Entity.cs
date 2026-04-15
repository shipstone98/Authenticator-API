using System;

namespace Shipstone.Authenticator.Api.Infrastructure.Entities;

/// <summary>
/// Represents an entity with a simple primary key. This class cannot be instantiated.
/// </summary>
/// <typeparam name="TId">The type of the ID of the entity.</typeparam>
public abstract class Entity<TId> where TId : struct
{
    /// <summary>
    /// Gets or initializes the date and time the entity was created.
    /// </summary>
    /// <value>The date and time the entity was created.</value>
    public DateTime Created { get; init; }

    /// <summary>
    /// Gets or initializes the ID of the entity.
    /// </summary>
    /// <value>The ID of the entity.</value>
    public TId Id { get; init; }

    /// <summary>
    /// Gets or sets the date and time the entity was last updated.
    /// </summary>
    /// <value>The date and time the entity was last updated.</value>
    public DateTime Updated { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity{TId}" /> class.
    /// </summary>
    protected Entity() { }
}
