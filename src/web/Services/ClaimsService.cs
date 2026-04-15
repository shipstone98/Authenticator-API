using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Shipstone.Authenticator.Api.Core;

namespace Shipstone.Authenticator.Api.Web.Services;

internal sealed class ClaimsService : IClaimsService
{
    private String? _emailAddress;
    private Nullable<Guid> _id;

    String IClaimsService.EmailAddress
    {
        get
        {
            if (this._emailAddress is null)
            {
                throw new UnauthorizedException("The current user is not authenticated.");
            }

            return this._emailAddress;
        }
    }

    Guid IClaimsService.Id
    {
        get
        {
            if (!this._id.HasValue)
            {
                throw new UnauthorizedException("The current user is not authenticated.");
            }

            return this._id.Value;
        }
    }

    internal void Authenticate(IEnumerable<Claim> claims)
    {
        Claim? emailAddressClaim =
            claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email));

        Claim? idClaim =
            claims.FirstOrDefault(c =>
                c.Type.Equals(ClaimTypes.NameIdentifier));

        if (
            emailAddressClaim is null
            || idClaim is null
            || !Guid.TryParse(idClaim.Value, out Guid id)
        )
        {
            this._emailAddress = null;
            this._id = null;
        }

        else
        {
            this._emailAddress = emailAddressClaim.Value;
            this._id = id;
        }
    }
}
