using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Shipstone.Extensions.Security;

using Shipstone.Authenticator.Api.Core;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Infrastructure.Data.Configuration;

internal sealed class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    private readonly IEncryptionService _encryption;

    internal UserConfiguration(IEncryptionService encryption) =>
        this._encryption = encryption;

    void IEntityTypeConfiguration<UserEntity>.Configure(EntityTypeBuilder<UserEntity> builder)
    {
        IEnumerable<Expression<Func<UserEntity, String?>>> protectedProperties =
            new Expression<Func<UserEntity, String?>>[]
        {
            u => u.EmailAddress,
            u => u.Forename,
            u => u.Surname
        };

        foreach (Expression<Func<UserEntity, String?>> protectedProperty in protectedProperties)
        {
            builder
                .Property(protectedProperty)
                .HasConversion(
                    i =>
                        i == null ? String.Empty : this._encryption.Encrypt(i),
                    o => this._encryption.Decrypt(o)
                );
        }

        builder
            .Property(u => u.Otp)
            .HasMaxLength(Constants.UserOtpMaxLength);

        builder
            .HasIndex(u => u.EmailAddressNormalized)
            .IsUnique();

        builder
            .HasMany<UserRefreshTokenEntity>()
            .WithOne()
            .HasForeignKey(urt => urt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
