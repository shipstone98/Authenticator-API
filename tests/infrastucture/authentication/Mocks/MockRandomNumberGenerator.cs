using System;
using System.Security.Cryptography;

using Shipstone.Test;

namespace Shipstone.Authenticator.Api.Infrastructure.AuthenticationTest.Mocks;

internal sealed class MockRandomNumberGenerator : RandomNumberGenerator
{
    internal SpanAction<byte> _getNonZeroBytesAction;

    public MockRandomNumberGenerator() =>
        this._getNonZeroBytesAction = _ => throw new NotImplementedException();

    protected sealed override void Dispose(bool disposing) =>
        throw new NotImplementedException();

    public sealed override bool Equals(Object? obj) =>
        throw new NotImplementedException();

    public sealed override void GetBytes(byte[] data) =>
        throw new NotImplementedException();

    public sealed override void GetBytes(byte[] data, int offset, int count) =>
        throw new NotImplementedException();

    public sealed override void GetBytes(Span<byte> data) =>
        throw new NotImplementedException();

    public sealed override int GetHashCode() =>
        throw new NotImplementedException();

    public sealed override void GetNonZeroBytes(byte[] data) =>
        throw new NotImplementedException();

    public sealed override void GetNonZeroBytes(Span<byte> data) =>
        this._getNonZeroBytesAction(data);

    public sealed override String? ToString() =>
        throw new NotImplementedException();
}
