using System;
using Microsoft.AspNetCore.Http;

namespace Shipstone.Authenticator.Api.WebTest.Mocks;

internal sealed class MockMiddlewareFactory : IMiddlewareFactory
{
    internal Func<Type, IMiddleware?> _createFunc;
    internal Action<IMiddleware> _releaseAction;

    public MockMiddlewareFactory()
    {
        this._createFunc = _ => throw new NotImplementedException();
        this._releaseAction = _ => throw new NotImplementedException();
    }

    IMiddleware? IMiddlewareFactory.Create(Type middlewareType) =>
        this._createFunc(middlewareType);

    void IMiddlewareFactory.Release(IMiddleware middleware) =>
        this._releaseAction(middleware);
}
