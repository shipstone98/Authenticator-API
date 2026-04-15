using System;

namespace Shipstone.Authenticator.Api.Web.Models.Account;

internal sealed class OtpAuthenticateRequest
{
#pragma warning disable CS8618
    internal String _emailAddress;
    internal String _otp;
#pragma warning restore CS8618

    public String EmailAddress
    {
        set => this._emailAddress = value;
    }

    public String Otp
    {
        set => this._otp = value;
    }
}
