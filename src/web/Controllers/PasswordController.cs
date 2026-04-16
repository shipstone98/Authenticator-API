using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Shipstone.Extensions.Identity;
using Shipstone.Utilities;

using Shipstone.Authenticator.Api.Core;
using Shipstone.Authenticator.Api.Core.Passwords;
using Shipstone.Authenticator.Api.Web.Models.Password;

namespace Shipstone.Authenticator.Api.Web.Controllers;

[Route("/api/[controller]/[action]")]
internal sealed class PasswordController(ILogger<PasswordController> logger)
    : ControllerBase<PasswordController>(logger)
{
    [ActionName("Reset")]
    [AllowAnonymous]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> ResetAsync(
        [FromServices] IPasswordResetHandler handler,
        [FromBody] ResetRequest request,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(request);
        return this.ResetAsyncCore(handler, request, cancellationToken);
    }

    private async Task<IActionResult> ResetAsyncCore(
        IPasswordResetHandler handler,
        ResetRequest request,
        CancellationToken cancellationToken
    )
    {
        await handler.HandleAsync(request._emailAddress, cancellationToken);

        this._logger.LogInformation(
            "{TimeStamp}: User {EmailAddress} reset password",
            DateTime.UtcNow,
            request._emailAddress
        );

        return this.NoContent();
    }

    [ActionName("Set")]
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public Task<IActionResult> SetAsync(
        [FromServices] IPasswordSetHandler handler,
        [FromBody] SetRequest request,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(request);
        return this.SetAsyncCore(handler, request, cancellationToken);
    }

    private async Task<IActionResult> SetAsyncCore(
        IPasswordSetHandler handler,
        SetRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await handler.HandleAsync(
                request._emailAddress,
                request._otp,
                request._password,
                cancellationToken
            );
        }

        catch (ForbiddenException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: Failed password set for user {EmailAddress} - OTP not valid or expired",
                DateTime.UtcNow,
                request._emailAddress
            );

            return this.Forbid();
        }

        catch (NotFoundException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: Failed password set for user {EmailAddress} - email address not found",
                DateTime.UtcNow,
                request._emailAddress
            );

            return this.Forbid();
        }

        catch (UserNotActiveException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: Failed password set for user {EmailAddress} - user not active",
                DateTime.UtcNow,
                request._emailAddress
            );

            return this.StatusCode(StatusCodes.Status410Gone);
        }

        this._logger.LogInformation(
            "{TimeStamp}: User {EmailAddress} set password",
            DateTime.UtcNow,
            request._emailAddress
        );

        return this.NoContent();
    }

    [ActionName("Update")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public Task<IActionResult> UpdateAsync(
        [FromServices] IPasswordUpdateHandler handler,
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
        IPasswordUpdateHandler handler,
        IClaimsService claims,
        UpdateRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await handler.HandleAsync(
                request._passwordCurrent,
                request._passwordNew,
                cancellationToken
            );
        }

        catch (IncorrectPasswordException ex)
        {
            this._logger.LogWarning(
                ex,
                "{TimeStamp}: Failed password update for user {EmailAddress} - password not correct",
                DateTime.UtcNow,
                claims.EmailAddress
            );

            return this.Forbid();
        }

        catch (UserNotActiveException ex)
        {
            this._logger.LogInformation(
                ex,
                "{TimeStamp}: Failed password update for user {EmailAddress} - user not active",
                DateTime.UtcNow,
                claims.EmailAddress
            );

            return this.StatusCode(StatusCodes.Status410Gone);
        }

        this._logger.LogInformation(
            "{TimeStamp}: User {EmailAddress} set password",
            DateTime.UtcNow,
            claims.EmailAddress
        );

        return this.NoContent();
    }
}
