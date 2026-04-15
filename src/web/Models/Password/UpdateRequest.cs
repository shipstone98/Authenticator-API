using System;

namespace Shipstone.Authenticator.Api.Web.Models.Password;

internal sealed class UpdateRequest
{
#pragma warning disable CS8618
    internal String _passwordCurrent;
    internal String _passwordNew;
#pragma warning restore CS8618

    public String PasswordCurrent
    {
        set => this._passwordCurrent = value;
    }

    public String PasswordNew
    {
        set => this._passwordNew = value;
    }
}
