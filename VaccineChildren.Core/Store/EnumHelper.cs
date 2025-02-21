using System.Reflection;

namespace VaccineChildren.Core.Store;

public static class EnumHelper
{
    public static string Name<T>(this T srcValue) =>
        GetCustomName(typeof(T).GetField(srcValue?.ToString() ?? string.Empty));

    private static string GetCustomName(FieldInfo? fi)
    {
        Type type = typeof(CustomName);

        Attribute? attr = null;
        if (fi is not null)
        {
            attr = Attribute.GetCustomAttribute(fi, type);
        }

        return (attr as CustomName)?.Name ?? string.Empty;
    }
    public static int Id(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<CustomId>();
        return attribute?.Id ?? 0;
    }
}