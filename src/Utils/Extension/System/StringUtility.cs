public static class StringUtility
{
    public static char[] ToCharArray(this string str, int startIndex)
    {
        return str.ToCharArray(startIndex, str.Length - startIndex);
    }
}