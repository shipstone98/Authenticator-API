using Shipstone.Authenticator.Api.Core.Accounts;

namespace Shipstone.Authenticator.Api.Web.Models.Account;

internal sealed class OtpAuthenticateResponse : AuthenticateResponseBase
{
    internal OtpAuthenticateResponse(IAuthenticateResult result) : base(result)
    { }
}
