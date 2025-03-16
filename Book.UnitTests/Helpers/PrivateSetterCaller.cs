using System.Linq.Expressions;
using System.Reflection;

namespace Book.UnitTests.Helpers;

public static class PrivateSetterCaller
{
    public static void SetPrivate<T, TValue>(this T instance, Expression<Func<T, TValue>> propertyExpression,
        TValue value)
    {
        instance.GetType().GetProperty(GetName(propertyExpression))?.SetValue(instance, value, null);
    }

    public static void SetPrivateField<T, TValue>(this T instance, string propertyName, TValue value)
    {
        var field = instance.GetType().GetField(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(instance,value);
    }

    private static string GetName<T, TValue>(Expression<Func<T, TValue>> exp)
    {
        var body = exp.Body as MemberExpression;

        if (body == null)
        {
            var ubody = (UnaryExpression)exp.Body;
            body = ubody.Operand as MemberExpression;
        }

        return body.Member.Name;
    }
}