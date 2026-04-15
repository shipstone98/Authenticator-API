using System;

namespace Shipstone.Authenticator.Api.Web.Models.Account;

internal sealed class AuthenticateRequest
{
#pragma warning disable CS8618
    internal String _emailAddress;
    internal String _password;
#pragma warning restore CS8618

    public String EmailAddress
    {
        set => this._emailAddress = value;
    }

    public String Password
    {
        set => this._password = value;
    }
}
