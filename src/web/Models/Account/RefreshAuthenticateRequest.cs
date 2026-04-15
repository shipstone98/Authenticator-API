using System;

namespace Shipstone.Authenticator.Api.Web.Models.Account;

internal sealed class RefreshAuthenticateRequest
{
#pragma warning disable CS8618
    internal String _refreshToken;
#pragma warning restore CS8618

    public String RefreshToken
    {
        set => this._refreshToken = value;
    }
}
