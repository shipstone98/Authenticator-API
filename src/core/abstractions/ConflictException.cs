using System;

namespace Shipstone.Authenticator.Api.Core;

/// <summary>
/// Represents the exception that is thrown when a requested resource is in a conflicting state.
/// </summary>
public class ConflictException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictException" /> class.
    /// </summary>
    public ConflictException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error, or <c>null</c>.</param>
    public ConflictException(String? message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error, or <c>null</c>.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or <c>null</c>.</param>
    public ConflictException(String? message, Exception? innerException)
        : base(message, innerException)
    { }
}
