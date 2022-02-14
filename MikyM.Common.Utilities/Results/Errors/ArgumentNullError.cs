// This file is part of Lisbeth.Bot project
using JetBrains.Annotations;
using MikyM.Common.Utilities.Results.Errors.Bases;

namespace MikyM.Common.Utilities.Results.Errors;

/// <summary>
/// Represents an error arising from an argument being null.
/// </summary>
/// <param name="Name">The name of the argument.</param>
/// <param name="message">The error message.</param>
/// <remarks>Used in place of <see cref="ArgumentNullException"/>.</remarks>
public record ArgumentNullError
(
    [InvokerParameterName] string Name,
    string message = "Value may not be null"
) : ArgumentError(Name, message);