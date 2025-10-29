
public struct IntRange
{
    public int min;
    public int max;

    public IntRange(int min, int max)
    {
        this.min = min;
        this.max = max;
    }

    public bool IsRange(int value)
    {
        return value >= min && value <= max;
    }
    public bool IsRangeInside(int value)
    {
        return value > min && value < max;
    }
    public int CountInside()
    {
        return max - min - 1;
    }
    public int Count()
    {
        return max - min + 1;
    }
}