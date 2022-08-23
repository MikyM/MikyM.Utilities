using System.Reflection;

namespace MikyM.Utilities.ExpressionHelpers;

public class PropOrField
{
    public readonly MemberInfo MemberInfo;

    public PropOrField(MemberInfo memberInfo)
    {
        if (memberInfo is not PropertyInfo && memberInfo is not FieldInfo)
        {
            throw new Exception(
                $"{nameof(memberInfo)} must either be {nameof(PropertyInfo)} or {nameof(FieldInfo)}");
        }

        MemberInfo = memberInfo;
    }

    public object? GetValue(object? source)
    {
        if (MemberInfo is PropertyInfo propertyInfo) return propertyInfo.GetValue(source);
        if (MemberInfo is FieldInfo fieldInfo) return fieldInfo.GetValue(source);

        return null;
    }

    public void SetValue(object target, object source)
    {
        if (MemberInfo is PropertyInfo propertyInfo) propertyInfo.SetValue(target, source);
        if (MemberInfo is FieldInfo fieldInfo) fieldInfo.SetValue(target, source);
    }

    public Type? GetMemberType()
    {
        if (MemberInfo is PropertyInfo propertyInfo) return propertyInfo.PropertyType;
        if (MemberInfo is FieldInfo fieldInfo) return fieldInfo.FieldType;

        return null;
    }
}
