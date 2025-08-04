using System.Runtime.CompilerServices;

public static class MemorySizeUtility
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float BToKB(float b)
    {
        return b / 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float BToMB(float b)
    {
        return b / 1024f / 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float BToGB(float b)
    {
        return b / 1024f / 1024f / 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float BToTB(float b)
    {
        return b / 1024f / 1024f / 1024f / 1024f;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float KBToB(float kb)
    {
        return kb * 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float KBToMB(float kb)
    {
        return kb / 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float KBToGB(float kb)
    {
        return kb / 1024f / 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float KBToTB(float kb)
    {
        return kb / 1024f / 1024f / 1024f;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float MBToB(float mb)
    {
        return mb * 1024f * 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float MBToKB(float mb)
    {
        return mb * 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float MBToGB(float mb)
    {
        return mb / 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float MBToTB(float mb)
    {
        return mb / 1024f / 1024f;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GBToB(float gb)
    {
        return gb * 1024f * 1024f * 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GBToKB(float gb)
    {
        return gb * 1024f * 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GBToMB(float gb)
    {
        return gb * 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GBToTB(float gb)
    {
        return gb / 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float TBToB(float tb)
    {
        return tb * 1024f * 1024f * 1024f * 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float TBToKB(float tb)
    {
        return tb * 1024f * 1024f * 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float TBToMB(float tb)
    {
        return tb * 1024f * 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float TBToGB(float tb)
    {
        return tb * 1024f;
    }
}