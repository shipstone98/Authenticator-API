using System;
using System.Text.RegularExpressions;

using Shipstone.Utilities;

namespace Shipstone.Authenticator.Api.Core.Services;

internal sealed partial class ValidationService : IValidationService
{
    private readonly int _ageMinimumYears;
    private readonly Regex _emailAddressPattern;
    private readonly Regex _forenamePattern;
    private readonly Regex _surnamePattern;

    public ValidationService()
    {
        this._ageMinimumYears = 18;

        this._emailAddressPattern =
            ValidationService.GenerateEmailAddressPattern();

        this._forenamePattern = ValidationService.GenerateForenamePattern();
        this._surnamePattern = ValidationService.GenerateSurnamePattern();
    }

    void IValidationService.ValidateBorn(DateOnly born, DateOnly today)
    {
        int ageYears;

        try
        {
            ageYears = born.GetAgeYears(today);
        }

        catch (ArgumentException)
        {
            ageYears = 0;
        }

        if (ageYears < this._ageMinimumYears)
        {
            throw new ArgumentException(
                $"{nameof (born)} is not a valid date of birth.",
                nameof (born)
            );
        }
    }

    String IValidationService.ValidateEmailAddress(String emailAddress) =>
        ValidationService.Validate(
            emailAddress,
            nameof (emailAddress),
            "email address",
            this._emailAddressPattern,
            Constants.UserEmailAddressMaxLength
        );

    String IValidationService.ValidateForename(String forename) =>
        ValidationService.Validate(
            forename,
            nameof (forename),
            "forename",
            this._forenamePattern,
            Constants.UserForenameMaxLength
        );

    String IValidationService.ValidateSurname(String surname) =>
        ValidationService.Validate(
            surname,
            nameof (surname),
            "surname",
            this._surnamePattern,
            Constants.UserSurnameMaxLength
        );

    [GeneratedRegex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.IgnoreCase
    )]
    private static partial Regex GenerateEmailAddressPattern();

    [GeneratedRegex(@"^([A-Z][a-z]+([A-Z][a-z]+)?((\-)[A-Z][a-z]+([A-Z][a-z]+))?)( ([A-Z][a-z]+([A-Z][a-z]+)?((\-)[A-Z][a-z]+([A-Z][a-z]+))?))*$")]
    private static partial Regex GenerateForenamePattern();

    [GeneratedRegex(@"^([A-Z][a-z]+([A-Z][a-z]+)?((\-| )[A-Z][a-z]+([A-Z][a-z]+))?)$")]
    private static partial Regex GenerateSurnamePattern();

    private static String Validate(
        String s,
        String paramName,
        String objectName,
        Regex pattern,
        int maxLength,
        int minLength = 1
    )
    {
        ArgumentNullException.ThrowIfNull(s, paramName);
        s = s.Trim();
        int length = s.Length;

        if (!pattern.IsMatch(s) || length > maxLength || length < minLength)
        {
            throw new ArgumentException(
                $"{paramName} is not a valid {objectName}.",
                paramName
            );
        }

        return s;
    }
}
