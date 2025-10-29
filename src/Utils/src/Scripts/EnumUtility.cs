using System;

namespace ManagerBot.Utils.Extension.System;
public static class EnumUtility
{
    public static string GetFlagsString<TEnum>(this TEnum target, params Span<TEnum> findEnums)
        where TEnum : struct, Enum
    {
        if (findEnums.Length == 0)
            findEnums = Enum.GetValues<TEnum>().AsSpan();

        Span<bool> hasFlags = stackalloc bool[findEnums.Length];

        int totalLength = 0;
        TEnum e;
        bool hasFlag;
        for (int i = 0; i < findEnums.Length; i++)
        {
            e = findEnums[i];

            totalLength += e.ToString().Length;

            hasFlag = target.HasFlag(e);
            hasFlags[i] = hasFlag;
            totalLength += hasFlag? 6 : 7; // ": true" or ": false"
        }
        totalLength += findEnums.Length - 1;

        Span<char> chars = stackalloc char[totalLength];
        int pos = 0;
        string enumString;
        for (int i = 0; i < findEnums.Length - 1; i++)
        {
            enumString = findEnums[i].ToString();
            enumString.AsSpan().CopyTo(chars[pos..]);
            pos += enumString.Length;

            if (target.HasFlag(findEnums[i]))
            {
                ": true".AsSpan().CopyTo(chars[pos..]);
                pos += 6;
            }
            else
            {
                ": false".AsSpan().CopyTo(chars[pos..]);
                pos += 7;
            }

            chars[pos++] = '\n';
        }
        enumString = findEnums[^1].ToString();
        enumString.AsSpan().CopyTo(chars[pos..]);
        pos += enumString.Length;

        if (target.HasFlag(findEnums[^1]))
        {
            ": true".AsSpan().CopyTo(chars[pos..]);
            pos += 6;
        }
        else
        {
            ": false".AsSpan().CopyTo(chars[pos..]);
            pos += 7;
        }

        return new string(chars);
    }
}
