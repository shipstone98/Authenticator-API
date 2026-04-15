using System;

namespace Shipstone.Authenticator.Api.Web.Models.Account;

internal sealed class RegisterRequest
{
#pragma warning disable CS8618
    internal DateOnly _born;
    internal String _emailAddress;
    internal String _forename;
    internal String _surname;
#pragma warning restore CS8618

    public DateOnly Born
    {
        set => this._born = value;
    }

    public String EmailAddress
    {
        set => this._emailAddress = value;
    }

    public String Forename
    {
        set => this._forename = value;
    }

    public String Surname
    {
        set => this._surname = value;
    }
}
