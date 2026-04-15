using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;

namespace Shipstone.Authenticator.Api.CoreTest.Mocks;

internal sealed class MockRepository : IRepository
{
    internal Action _saveAction;
    internal Func<IUserRefreshTokenRepository> _userRefreshTokensFunc;
    internal Func<IUserRepository> _usersFunc;

    IUserRefreshTokenRepository IRepository.UserRefreshTokens =>
        this._userRefreshTokensFunc();

    IUserRepository IRepository.Users => this._usersFunc();

    public MockRepository()
    {
        this._saveAction = () => throw new NotImplementedException();

        this._userRefreshTokensFunc = () =>
            throw new NotImplementedException();

        this._usersFunc = () => throw new NotImplementedException();
    }

    Task IRepository.SaveAsync(CancellationToken cancellationToken)
    {
        this._saveAction();
        return Task.CompletedTask;
    }
}
