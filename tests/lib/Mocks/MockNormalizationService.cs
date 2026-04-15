using System;

using Shipstone.Extensions.Security;

namespace Shipstone.Authenticator.Api.Test.Mocks;

public sealed class MockNormalizationService : INormalizationService
{
    public Func<String, String> _normalizeFunc;

    public MockNormalizationService() =>
        this._normalizeFunc = _ => throw new NotImplementedException();

    String INormalizationService.Normalize(String s) => this._normalizeFunc(s);
}
