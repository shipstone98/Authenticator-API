using System;

using Shipstone.Authenticator.Api.Infrastructure.Entities;

namespace Shipstone.Authenticator.Api.Core;

internal sealed class User : IUser
{
    private readonly UserEntity _user;

    DateOnly IUser.Born => this._user.Born;
    DateTime IUser.Consented => this._user.Consented;
    DateTime IUser.Created => this._user.Created;
    String IUser.EmailAddress => this._user.EmailAddress;
    String IUser.Forename => this._user.Forename;
    Guid IUser.Id => this._user.Id;
    String IUser.Surname => this._user.Surname;
    DateTime IUser.Updated => this._user.Updated;

    internal User(UserEntity user) => this._user = user;
}
