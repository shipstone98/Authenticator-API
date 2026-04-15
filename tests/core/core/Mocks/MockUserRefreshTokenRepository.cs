using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.CoreTest.Mocks;

internal sealed class MockUserRefreshTokenRepository
    : IUserRefreshTokenRepository
{
    internal Action<UserRefreshTokenEntity> _createAction;
    internal Action<UserRefreshTokenEntity> _deleteAction;
    internal Func<Guid, UserRefreshTokenEntity[]> _listForUserFunc;
    internal Func<String, UserRefreshTokenEntity?> _retrieveFunc;

    internal MockUserRefreshTokenRepository()
    {
        this._createAction = _ => throw new NotImplementedException();
        this._deleteAction = _ => throw new NotImplementedException();
        this._listForUserFunc = _ => throw new NotImplementedException();
        this._retrieveFunc = _ => throw new NotImplementedException();
    }

    Task IUserRefreshTokenRepository.CreateAsync(
        UserRefreshTokenEntity userRefreshToken,
        CancellationToken cancellationToken
    )
    {
        this._createAction(userRefreshToken);
        return Task.CompletedTask;
    }

    Task IUserRefreshTokenRepository.DeleteAsync(
        UserRefreshTokenEntity userRefreshToken,
        CancellationToken cancellationToken
    )
    {
        this._deleteAction(userRefreshToken);
        return Task.CompletedTask;
    }

    Task<UserRefreshTokenEntity[]> IUserRefreshTokenRepository.ListForUserAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        UserRefreshTokenEntity[] userRefreshTokens =
            this._listForUserFunc(userId);

        return Task.FromResult(userRefreshTokens);
    }

    Task<UserRefreshTokenEntity?> IUserRefreshTokenRepository.RetrieveAsync(
        String val,
        CancellationToken cancellationToken
    )
    {
        UserRefreshTokenEntity? result = this._retrieveFunc(val);
        return Task.FromResult(result);
    }
}
