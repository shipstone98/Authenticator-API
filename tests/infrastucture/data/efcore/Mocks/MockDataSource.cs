using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.EntityFrameworkCore;

using Shipstone.Authenticator.Api.Infrastructure.Data;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Infrastructure.DataTest.Mocks;

internal sealed class MockDataSource : IDataSource
{
    internal Action _saveAction;
    internal Func<IDataSet<UserRefreshTokenEntity>> _userRefreshTokensFunc;
    internal Func<IDataSet<UserEntity>> _usersFunc;

    IDataSet<UserRefreshTokenEntity> IDataSource.UserRefreshTokens =>
        this._userRefreshTokensFunc();

    IDataSet<UserEntity> IDataSource.Users => this._usersFunc();

    public MockDataSource()
    {
        this._saveAction = () => throw new NotImplementedException();

        this._userRefreshTokensFunc = () =>
            throw new NotImplementedException();

        this._usersFunc = () => throw new NotImplementedException();
    }

    Task IDataSource.SaveAsync(CancellationToken cancellationToken)
    {
        this._saveAction();
        return Task.CompletedTask;
    }
}
