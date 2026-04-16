using System;

namespace Shipstone.Authenticator.Api.Web.Models.User;

internal sealed class UpdateRequest
{
#pragma warning disable CS8618
    internal String _forename;
    internal String _surname;
#pragma warning restore CS8618

    public String Forename
    {
        set => this._forename = value;
    }

    public String Surname
    {
        set => this._surname = value;
    }
}
