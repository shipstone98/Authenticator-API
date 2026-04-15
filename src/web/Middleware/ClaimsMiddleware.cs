using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Shipstone.Authenticator.Api.Web.Services;

namespace Shipstone.Authenticator.Api.Web.Middleware;

internal sealed class ClaimsMiddleware : IMiddleware
{
    private readonly ClaimsService _claims;

    public ClaimsMiddleware(ClaimsService claims)
    {
        ArgumentNullException.ThrowIfNull(claims);
        this._claims = claims;
    }

    Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);
        this._claims.Authenticate(context.User.Claims);
        return next(context);
    }
}
