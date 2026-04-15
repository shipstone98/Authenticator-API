using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;

internal sealed class Repository : IRepository
{
    private readonly IDataSource _dataSource;
    private readonly IServiceProvider _provider;

    IUserRefreshTokenRepository IRepository.UserRefreshTokens =>
        this._provider.GetRequiredService<IUserRefreshTokenRepository>();

    IUserRepository IRepository.Users =>
        this._provider.GetRequiredService<IUserRepository>();

    public Repository(IServiceProvider provider, IDataSource dataSource)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(dataSource);
        this._dataSource = dataSource;
        this._provider = provider;
    }

    Task IRepository.SaveAsync(CancellationToken cancellationToken) =>
        this._dataSource.SaveAsync(cancellationToken);
}
