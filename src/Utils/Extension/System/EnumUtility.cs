using System.Text;

namespace System;
public static class EnumUtility
{
    public static string GetFlagString<TEnum>(this TEnum target, TEnum findEnum) 
        where TEnum : Enum
    {
        return $"{Enum.GetName(typeof(TEnum), findEnum)}: {target.HasFlag(findEnum)}";
    }
    public static string GetFlagsString<TEnum>(this TEnum target, params TEnum[] findEnums) 
        where TEnum : Enum
    {
        StringBuilder stringBuilder = new();
        foreach (TEnum findEnum in findEnums)
        {
            stringBuilder.AppendLine(GetFlagString(target, findEnum));
        }
        return stringBuilder.ToString();
    }
}