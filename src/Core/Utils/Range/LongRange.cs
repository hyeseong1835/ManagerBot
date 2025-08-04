
public struct LongRange
{
    public long min;
    public long max;

    public LongRange(long min, long max)
    {
        this.min = min;
        this.max = max;
    }

    public bool IsRangeOrSame(long value)
    {
        return value >= min && value <= max;
    }
    public bool IsRangeInside(long value)
    {
        return value > min && value < max;
    }
    public long CountInside()
    {
        return max - min - 1;
    }
}