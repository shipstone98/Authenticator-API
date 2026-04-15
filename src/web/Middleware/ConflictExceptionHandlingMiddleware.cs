using Shipstone.AspNetCore.Http;

using Shipstone.Authenticator.Api.Core;

namespace Shipstone.Authenticator.Api.Web.Middleware;

#warning Not tested
internal sealed class ConflictExceptionHandlingMiddleware
    : ExceptionHandlingMiddleware<ConflictException>
{
    internal ConflictExceptionHandlingMiddleware(int statusCode)
        : base(statusCode)
    { }
}
