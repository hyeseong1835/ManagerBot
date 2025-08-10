using System.Runtime.CompilerServices;

public static class MemorySizeUtility
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float BToKiB(float b)
    {
        return b / 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float BToMiB(float b)
    {
        return b / 1024f / 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float BToGiB(float b)
    {
        return b / 1024f / 1024f / 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float BToTiB(float b)
    {
        return b / 1024f / 1024f / 1024f / 1024f;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float KiBToB(float KiB)
    {
        return KiB * 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float KiBToMiB(float KiB)
    {
        return KiB / 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float KiBToGiB(float KiB)
    {
        return KiB / 1024f / 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float KiBToTiB(float KiB)
    {
        return KiB / 1024f / 1024f / 1024f;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float MiBToB(float MiB)
    {
        return MiB * 1024f * 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float MiBToKiB(float MiB)
    {
        return MiB * 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float MiBToGiB(float MiB)
    {
        return MiB / 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float MiBToTiB(float MiB)
    {
        return MiB / 1024f / 1024f;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GiBToB(float GiB)
    {
        return GiB * 1024f * 1024f * 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GiBToKiB(float GiB)
    {
        return GiB * 1024f * 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GiBToMiB(float GiB)
    {
        return GiB * 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GiBToTiB(float GiB)
    {
        return GiB / 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float TiBToB(float TiB)
    {
        return TiB * 1024f * 1024f * 1024f * 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float TiBToKiB(float TiB)
    {
        return TiB * 1024f * 1024f * 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float TiBToMiB(float TiB)
    {
        return TiB * 1024f * 1024f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float TiBToGiB(float TiB)
    {
        return TiB * 1024f;
    }
}