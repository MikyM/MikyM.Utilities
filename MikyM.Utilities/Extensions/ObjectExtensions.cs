namespace MikyM.Utilities.Extensions;

public static class ObjectExtensions
{
    public static T CastObject<T>(this object input)
    {
        return (T)input;
    }

    public static T ConvertObject<T>(this object input)
    {
        return (T)Convert.ChangeType(input, typeof(T));
    }
    public static T ConvertObject<T>(this object input, T type) where T : Type
    {
        return (T)Convert.ChangeType(input, type);
    }
}