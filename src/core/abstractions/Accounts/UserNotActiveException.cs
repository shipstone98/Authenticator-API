using System;

namespace Shipstone.Authenticator.Api.Core.Accounts;

/// <summary>
/// Represents the exception that is thrown when the current user is not active.
/// </summary>
public class UserNotActiveException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserNotActiveException" /> class.
    /// </summary>
    public UserNotActiveException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserNotActiveException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error, or <c>null</c>.</param>
    public UserNotActiveException(String? message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserNotActiveException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error, or <c>null</c>.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or <c>null</c>.</param>
    public UserNotActiveException(String? message, Exception? innerException)
        : base(message, innerException)
    { }
}
