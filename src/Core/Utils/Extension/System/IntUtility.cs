
namespace System;
public static class IntUtility
{
    public static bool IsRangeOrSame(int value, int min, int max)
    {
        return value >= min && value <= max;
    }
    public static bool IsRangeInside(int value, int min, int max)
    {
        return value > min && value < max;
    }
    public static int[] CreateRangeArray(int start, int length)
    {
        int[] range = new int[length];

        for (int i = 0; i < length; i++)
        {
            range[i] = start + i;
        }
        return range;
    }
    public static int[] CreateRangeArray(int length)
    {
        int[] range = new int[length];

        for (int i = 0; i < length; i++)
        {
            range[i] = i;
        }
        return range;
    }
}