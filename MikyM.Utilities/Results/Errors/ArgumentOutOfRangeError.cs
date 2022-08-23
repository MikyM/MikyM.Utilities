using MikyM.Utilities.Results.Errors.Bases;

namespace MikyM.Utilities.Results.Errors;

/// <summary>
/// Represents an error arising from an argument being outside of an expected range.
/// </summary>
/// <param name="Name">The name of the argument.</param>
/// <param name="Message">The error message.</param>
/// <remarks>Used in place of <see cref="ArgumentOutOfRangeException"/>.</remarks>
[PublicAPI]
public record ArgumentOutOfRangeError
(
    [InvokerParameterName] string Name,
    string Message = "Value was outside of the expected range"
) : ArgumentError(Name, Message);
