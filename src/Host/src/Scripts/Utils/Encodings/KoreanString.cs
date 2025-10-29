namespace ManagerBot.Utils.Encodings;

public static class KoreanString
{
    public static readonly int KoreanCharCount = CharRange.KoreanRange.Count(); //11172;
    public const int UlongKoreanCharSize = 5;
    public const int IntKoreanCharSize = 3;

    #region Ulong

    public static string UlongToKoreanString(ulong id)
        => new string(UlongToKoreanCharArray(id));
    public static char[] UlongToKoreanCharArray(ulong id)
    {
        char[] result = new char[UlongKoreanCharSize];
        int i = 0;
        for (; i < UlongKoreanCharSize; i++)
        {
            if (id == 0) break;

            int remainder = (int)(id % (ulong)KoreanCharCount);

            result[i] = (char)(remainder + CharRange.KoreanRange.min);
            id /= (ulong)KoreanCharCount;
        }
        for (; i < UlongKoreanCharSize; i++)
        {
            result[i] = (char)CharRange.KoreanRange.min;
        }

        return result;
    }

    public static ulong KoreanStringToUlongId(string str)
        => KoreanCharArrayToUlong(str.ToCharArray());
    public static ulong KoreanCharArrayToUlong(char[] chars)
    {
        ulong result = 0;
        ulong multiplier = 1;

        for (int i = 0; i < chars.Length; i++)
        {
            if (i >= UlongKoreanCharSize) break;

            char c = chars[i];
            if (!CharRange.KoreanRange.IsRangeOrSame(c)) continue;

            int digit = c - CharRange.KoreanRange.min;
            result += (ulong)digit * multiplier;
            multiplier *= (ulong)KoreanCharCount;
        }

        return result;
    }

    #endregion
}
