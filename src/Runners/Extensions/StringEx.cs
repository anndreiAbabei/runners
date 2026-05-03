namespace Runners.Extensions;

public static class StringEx
{
    public static string Default(this string value, string defaultValue)
    {
        return string.IsNullOrEmpty(value) 
                   ? defaultValue 
                   : value;
    }
}
