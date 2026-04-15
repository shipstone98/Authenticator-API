using Shipstone.Authenticator.Api.Core.Accounts;

namespace Shipstone.Authenticator.Api.Web.Models.Account;

internal sealed class RefreshAuthenticateResponse : AuthenticateResponseBase
{
    internal RefreshAuthenticateResponse(IAuthenticateResult result)
        : base(result)
    { }
}
