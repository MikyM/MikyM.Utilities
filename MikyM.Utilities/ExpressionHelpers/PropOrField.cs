using System.Reflection;

namespace MikyM.Utilities.ExpressionHelpers;

/// <summary>
/// 
/// </summary>
public class PropOrField
{
    /// <summary>
    /// 
    /// </summary>
    public readonly MemberInfo MemberInfo;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="memberInfo"></param>
    /// <exception cref="Exception"></exception>
    public PropOrField(MemberInfo memberInfo)
    {
        if (memberInfo is not PropertyInfo && memberInfo is not FieldInfo)
        {
            throw new Exception(
                $"{nameof(memberInfo)} must either be {nameof(PropertyInfo)} or {nameof(FieldInfo)}");
        }

        MemberInfo = memberInfo;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public object? GetValue(object? source)
    {
        if (MemberInfo is PropertyInfo propertyInfo) return propertyInfo.GetValue(source);
        if (MemberInfo is FieldInfo fieldInfo) return fieldInfo.GetValue(source);

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="source"></param>
    public void SetValue(object target, object source)
    {
        if (MemberInfo is PropertyInfo propertyInfo) propertyInfo.SetValue(target, source);
        if (MemberInfo is FieldInfo fieldInfo) fieldInfo.SetValue(target, source);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Type? GetMemberType()
    {
        if (MemberInfo is PropertyInfo propertyInfo) return propertyInfo.PropertyType;
        if (MemberInfo is FieldInfo fieldInfo) return fieldInfo.FieldType;

        return null;
    }
}
