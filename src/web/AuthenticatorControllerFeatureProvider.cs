using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

using Shipstone.Authenticator.Api.Web.Controllers;

namespace Shipstone.Authenticator.Api.Web;

internal sealed class AuthenticatorControllerFeatureProvider
    : ControllerFeatureProvider
{
    private readonly IReadOnlySet<Type> _types;

    internal AuthenticatorControllerFeatureProvider() =>
        this._types = new HashSet<Type>
        {
            typeof (AccountController),
            typeof (PasswordController),
            typeof (UserController)
        };

    protected sealed override bool IsController(TypeInfo typeInfo)
    {
        ArgumentNullException.ThrowIfNull(typeInfo);
        return this._types.Contains(typeInfo);
    }
}
