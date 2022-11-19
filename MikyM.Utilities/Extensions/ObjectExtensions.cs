using System.Diagnostics.CodeAnalysis;

namespace MikyM.Utilities.Extensions;

/// <summary>
/// 
/// </summary>
[PublicAPI]
public static class ObjectExtensions
{
    /// <summary>
    /// Casts the current object to another type.
    /// </summary>
    /// <param name="input">Object to be cast.</param>
    /// <typeparam name="T">Type to which the object should be cast.</typeparam>
    /// <returns>The cast object.</returns>
    public static T CastObject<T>(this object input)
        => (T)input;
    
    /// <summary>
    /// Casts the current object to another type.
    /// </summary>
    /// <param name="input">Object to be cast.</param>
    /// <typeparam name="T">Type to which the object should be cast.</typeparam>
    /// <returns>The cast object.</returns>
    public static T CastTo<T>(this object input)
        => CastObject<T>(input);

    /// <summary>
    /// Safe casts the current object to another type.
    /// </summary>
    /// <param name="input">Object to be cast.</param>
    /// <typeparam name="T">Type to which the object should be cast.</typeparam>
    /// <returns>The cast object.</returns>
    public static T? SafeCastToValueType<T>(this object input) where T : struct
    {
        try
        {
            return (T?)input;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Safe casts the current object to another type.
    /// </summary>
    /// <param name="input">Object to be cast.</param>
    /// <typeparam name="T">Type to which the object should be cast.</typeparam>
    /// <returns>The cast object.</returns>
    public static T? SafeCastObject<T>(this object input) where T : class?
        => input as T;
    
    /// <summary>
    /// Safe casts the current object to another type.
    /// </summary>
    /// <param name="input">Object to be cast.</param>
    /// <typeparam name="T">Type to which the object should be cast.</typeparam>
    /// <returns>The cast object.</returns>
    public static T? SafeCastTo<T>(this object input) where T : class?
        => SafeCastObject<T>(input);

    /// <summary>
    /// Tries to cast the current object to another type.
    /// </summary>
    /// <param name="input">Object to be cast.</param>
    /// <param name="cast">The cast object if any.</param>
    /// <typeparam name="T">Type to which the object should be cast.</typeparam>
    /// <returns>True if casting was possible, otherwise false.</returns>
    public static bool TryCastObject<T>(this object input, [NotNullWhen(true)] out T? cast) where T : class?
    {
        cast = input as T;
        return cast is not null;
    }

    /// <summary>
    /// Tries to cast the current object to another type.
    /// </summary>
    /// <param name="input">Object to be cast.</param>
    /// <param name="cast">The cast object if any.</param>
    /// <typeparam name="T">Type to which the object should be cast.</typeparam>
    /// <returns>True if casting was possible, otherwise false.</returns>
    public static bool TryCastTo<T>(this object input, [NotNullWhen(true)] out T? cast) where T : class?
        => TryCastObject(input, out cast);

    /// <summary>
    /// Uses <see cref="Convert"/> class under the hood.
    /// </summary>
    /// <param name="input"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ConvertObject<T>(this object input)
        => (T)Convert.ChangeType(input, typeof(T));
    
    /// <summary>
    /// Uses <see cref="Convert"/> class under the hood.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="type"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ConvertObject<T>(this object input, T type) where T : Type
        => (T)Convert.ChangeType(input, type);
}
