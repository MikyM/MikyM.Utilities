using JetBrains.Annotations;

namespace MikyM.Common.Utilities.Results;

/// <summary>
/// Represents the public API of an interface.
/// </summary>
[PublicAPI]
public interface IResult
{
    /// <summary>
    /// Gets a value indicating whether the result was successful.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets the error, if any.
    /// </summary>
    IResultError? Error { get; }

    /// <summary>
    /// Gets the inner result, if any.
    /// </summary>
    IResult? Inner { get; }

    /// <summary>
    /// Tries to get the underlying inner result.
    /// </summary>
    /// <returns>true if the result contains an entity; otherwise, false.</returns>
    bool TryGetInner(out IResult? inner);

    /// <summary>
    /// Tries to get the underlying error result.
    /// </summary>
    /// <returns>true if the result contains an entity; otherwise, false.</returns>
    bool TryGetError(out IResultError? error);
}

/// <summary>
/// Represents the public API of an interface.
/// </summary>
[PublicAPI]
public interface IResult<TEntity> : IResult
{
    /// <summary>
    /// Tries to get the underlying entity returned by the result.
    /// </summary>
    /// <returns>true if the result contains an entity; otherwise, false.</returns>
    bool TryGetEntity(out TEntity? entity);

    /// <summary>
    /// Gets the entity returned by the result.
    /// </summary>
    TEntity? Entity { get; }
}