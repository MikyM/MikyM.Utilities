namespace MikyM.Common.Utilities.Results.Errors;

/// <summary>
/// Represents a failure to find something that was searched for.
/// </summary>
/// <param name="Message">The custom message to provide.</param>
[PublicAPI]
public record NotFoundError(string Message = "The searched-for entity was not found.") : ResultError(Message);
