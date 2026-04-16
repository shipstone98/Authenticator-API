using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Shipstone.Authenticator.Api.Web.Controllers;

[ApiController]
[Authorize]
[Route("/api/[controller]")]
internal abstract class ControllerBase<TCategoryName> : ControllerBase
    where TCategoryName : ControllerBase<TCategoryName>
{
    private protected readonly ILogger<TCategoryName> _logger;

    private protected ControllerBase(ILogger<TCategoryName> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        this._logger = logger;
    }
}
