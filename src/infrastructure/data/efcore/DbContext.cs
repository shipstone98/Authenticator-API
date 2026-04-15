using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Shipstone.EntityFrameworkCore;
using Shipstone.Extensions.Security;

using Shipstone.Authenticator.Api.Infrastructure.Data.Configuration;
using Shipstone.Authenticator.Api.Infrastructure.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Shipstone.Authenticator.Api.Infrastructure.Data;

/// <summary>
/// Represents a database. This class cannot be instantiated.
/// </summary>
/// <typeparam name="TContext">The type of the database context.</typeparam>
public abstract class DbContext<TContext> : DbContext, IDataSource
    where TContext : DbContext<TContext>
{
    private readonly IEncryptionService _encryption;
    private readonly DbSet<UserRefreshTokenEntity> _userRefreshTokens;
    private readonly DbSet<UserEntity> _users;

    public DbSet<UserRefreshTokenEntity> UserRefreshTokens =>
        this._userRefreshTokens;

    public DbSet<UserEntity> Users => this._users;

    IDataSet<UserRefreshTokenEntity> IDataSource.UserRefreshTokens =>
        new DataSet<UserRefreshTokenEntity>(this._userRefreshTokens);

    IDataSet<UserEntity> IDataSource.Users =>
        new DataSet<UserEntity>(this._users);

    protected DbContext(
        DbContextOptions<TContext> options,
        IEncryptionService encryption
    ) : base(options)
    {
        ArgumentNullException.ThrowIfNull(encryption);
        this._encryption = encryption;
        this._userRefreshTokens = this.Set<UserRefreshTokenEntity>();
        this._users = this.Set<UserEntity>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        IEntityTypeConfiguration<UserEntity> userConfiguration =
            new UserConfiguration(this._encryption);

        modelBuilder.ApplyConfiguration(userConfiguration);
    }

    /// <inheritdoc />
    public sealed override DbSet<TEntity> Set<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties | DynamicallyAccessedMemberTypes.Interfaces)] TEntity>() =>
        base.Set<TEntity>();

    Task IDataSource.SaveAsync(CancellationToken cancellationToken) =>
        this.SaveChangesAsync(cancellationToken);
}
