using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Shipstone.Utilities;

using Shipstone.Authenticator.Api.Core;
using Shipstone.Authenticator.Api.Core.Users;
using Shipstone.Authenticator.Api.Web.Models.User;

namespace Shipstone.Authenticator.Api.Web.Controllers;

internal sealed class UserController(ILogger<UserController> logger)
    : ControllerBase<UserController>(logger)
{
    [ActionName("Update")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public Task<IActionResult> UpdateAsync(
        [FromServices] IUserUpdateHandler handler,
        [FromServices] IClaimsService claims,
        [FromBody] UpdateRequest request,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(request);

        return this.UpdateAsyncCore(
            handler,
            claims,
            request,
            cancellationToken
        );
    }

    private async Task<IActionResult> UpdateAsyncCore(
        IUserUpdateHandler handler,
        IClaimsService claims,
        UpdateRequest request,
        CancellationToken cancellationToken
    )
    {
        IUser user;

        try
        {
            user =
                await handler.HandleAsync(
                    request._forename,
                    request._surname,
                    cancellationToken
                );
        }

        catch (UserNotActiveException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: Failed to update user {EmailAddress} - user not active",
                DateTime.UtcNow,
                claims.EmailAddress
            );

            return this.StatusCode(StatusCodes.Status410Gone);
        }

        this._logger.LogInformation(
            "{TimeStamp}: User {EmailAddress} updated",
            user.Updated,
            user.EmailAddress
        );

        return this.NoContent();
    }
}
