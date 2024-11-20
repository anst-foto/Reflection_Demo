using System.Reflection;
using System.Text;

namespace Reflection_Demo.Csv_Serializer;

public static class CsvSerializer
{
    public static string Serialize<T>(T obj, string separator = ";")
    {
        //var type = typeof(T);
        var type = obj.GetType();
        var separatorAttr = type.GetCustomAttribute<CsvSeparatorAttribute>();
        if (separatorAttr != null)
        {
            separator = separatorAttr.Separator;
        }
        
        var sb = new StringBuilder();

        var interfaces = type.GetInterfaces();
        foreach (var @interface in interfaces)
        {
            if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var enumerable = obj as IEnumerable<object>;
                foreach (var o in enumerable)
                {
                    sb.Append(Serialize(o, separator));
                    sb.Append(Environment.NewLine);
                }
            }
            //TODO добавить обработку Dictionary<T1, T2>
        }
        
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            var attr = field.GetCustomAttribute<CsvIgnoreAttribute>();
            if (attr is { Ignore: true }) continue;

            sb.Append(field.GetValue(obj));
            sb.Append(separator);
        }

        sb.Remove(sb.Length - 1, 1);

        return sb.ToString();
    } 
}