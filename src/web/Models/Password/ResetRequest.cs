using System;

namespace Shipstone.Authenticator.Api.Web.Models.Password;

internal sealed class ResetRequest
{
#pragma warning disable CS8618
    internal String _emailAddress;
#pragma warning restore CS8618

    public String EmailAddress
    {
        set => this._emailAddress = value;
    }
}
