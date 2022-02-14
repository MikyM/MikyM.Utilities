namespace MikyM.Common.Utilities.Optionals;

/// <summary>
/// Defines basic functionality for an optional.
/// </summary>
public interface IOptional
{
    /// <summary>
    /// Gets a value indicating whether the optional contains a value.
    /// </summary>
    bool HasValue { get; }

    /// <summary>
    /// Determines whether the option has a defined value; that is, whether it both has a value and that value is
    /// non-null.
    /// </summary>
    /// <returns>true if the optional has a value and that value is non-null; otherwise, false.</returns>
    bool IsDefined();
}