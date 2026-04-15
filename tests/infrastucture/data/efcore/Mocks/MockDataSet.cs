using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.EntityFrameworkCore;

using Shipstone.Test.Mocks;

namespace Shipstone.Authenticator.Api.Infrastructure.DataTest.Mocks;

internal sealed class MockDataSet<TEntity>
    : MockAsyncQueryable<TEntity>, IDataSet<TEntity>
{
    internal Action<TEntity, DataEntityState> _setStateAction;

    internal MockDataSet(IQueryable<TEntity> queryable) : base(queryable) =>
        this._setStateAction = (_, _) => throw new NotImplementedException();

    Task IDataSet<TEntity>.SetStateAsync(
        TEntity entity,
        DataEntityState state,
        CancellationToken cancellationToken
    )
    {
        this._setStateAction(entity, state);
        return Task.CompletedTask;
    }
}
