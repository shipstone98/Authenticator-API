using System;

using Shipstone.Authenticator.Api.Core.Accounts;

namespace Shipstone.Authenticator.Api.Web.Models.Account;

internal abstract class AuthenticateResponseBase
{
    private readonly String _accessToken;
    private readonly String _refreshToken;

    public String AccessToken => this._accessToken;
    public String RefreshToken => this._refreshToken;

    private protected AuthenticateResponseBase(IAuthenticateResult result)
    {
        this._accessToken = result.AccessToken;
        this._refreshToken = result.RefreshToken;
    }
}
