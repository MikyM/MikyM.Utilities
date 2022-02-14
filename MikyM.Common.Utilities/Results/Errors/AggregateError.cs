namespace MikyM.Common.Utilities.Results.Errors;

/// <summary>
/// Represents a set of errors produced by an operation.
/// </summary>
/// <param name="Errors">The errors.</param>
/// <param name="Message">The custom error message, if any.</param>
/// <remarks>Used in place of <see cref="AggregateException"/>.</remarks>
public record AggregateError
(
    IReadOnlyCollection<IResult> Errors,
    string Message = "One or more errors occurred."
) : ResultError(Message)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateError"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errors">The errors.</param>
    public AggregateError(string message, params IResult[] errors)
        : this(errors, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateError"/> class.
    /// </summary>
    /// <param name="errors">The errors.</param>
    public AggregateError(params IResult[] errors)
        : this((IReadOnlyCollection<IResult>)errors)
    {
    }
}