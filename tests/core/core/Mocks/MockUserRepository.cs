using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Authenticator.Api.Infrastructure.Data.Repositories;
using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.CoreTest.Mocks;

internal sealed class MockUserRepository : IUserRepository
{
    internal Action<UserEntity> _createAction;
    internal Func<Guid, UserEntity?> _retrieve_GuidFunc;
    internal Func<String, UserEntity?> _retrieve_StringFunc;
    internal Action<UserEntity> _updateAction;

    public MockUserRepository()
    {
        this._createAction = _ => throw new NotImplementedException();
        this._retrieve_GuidFunc = _ => throw new NotImplementedException();
        this._retrieve_StringFunc = _ => throw new NotImplementedException();
        this._updateAction = _ => throw new NotImplementedException();
    }

    Task IUserRepository.CreateAsync(
        UserEntity user,
        CancellationToken cancellationToken
    )
    {
        this._createAction(user);
        return Task.CompletedTask;
    }

    Task<UserEntity?> IUserRepository.RetrieveAsync(
        String emailAddress,
        CancellationToken cancellationToken
    )
    {
        UserEntity? result = this._retrieve_StringFunc(emailAddress);
        return Task.FromResult(result);
    }

    Task<UserEntity?> IUserRepository.RetrieveAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        UserEntity? result = this._retrieve_GuidFunc(id);
        return Task.FromResult(result);
    }

    Task IUserRepository.UpdateAsync(
        UserEntity user,
        CancellationToken cancellationToken
    )
    {
        this._updateAction(user);
        return Task.CompletedTask;
    }
}
