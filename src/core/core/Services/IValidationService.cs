using System;

namespace Shipstone.Authenticator.Api.Core.Services;

internal interface IValidationService
{
    void ValidateBorn(DateOnly born, DateOnly today);
    String ValidateEmailAddress(String emailAddress);
    String ValidateForename(String forename);
    String ValidateSurname(String surname);
}
