namespace MikyM.Utilities.Results;

/// <summary>
/// Acts as a base class for result errors.
/// </summary>
[PublicAPI]
public abstract record ResultError(string Message) : IResultError;
