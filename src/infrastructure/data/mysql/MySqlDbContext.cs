using Microsoft.EntityFrameworkCore;

using Shipstone.Extensions.Security;

namespace Shipstone.Authenticator.Api.Infrastructure.Data.MySql;

internal sealed class MySqlDbContext : DbContext<MySqlDbContext>
{
    internal MySqlDbContext(
        DbContextOptions<MySqlDbContext> options,
        IEncryptionService encryption
    )
        : base(options, encryption)
    { }
}
