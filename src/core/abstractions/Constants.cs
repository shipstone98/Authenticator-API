namespace Shipstone.Authenticator.Api.Core;

/// <summary>
/// Defines constants for the well-known Authenticator constants.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Specifies the maximum length of a user email address. This field is constant.
    /// </summary>
    public const int UserEmailAddressMaxLength = 256;

    /// <summary>
    /// Specifies the maximum length of a user forename. This field is constant.
    /// </summary>
    public const int UserForenameMaxLength = 128;

    /// <summary>
    /// Specifies the maximum length of a user surname. This field is constant.
    /// </summary>
    public const int UserSurnameMaxLength = 64;

    /// <summary>
    /// Specifies the maximum length of a user one-time passcode (OTP). This field is constant.
    /// </summary>
    public const int UserOtpMaxLength = 6;
}
