
public struct CharRange
{
    public static CharRange EnglishLowerCaseRange = new('a', 'z');
    public static CharRange EnglishUpperCaseRange = new('A', 'Z');
    public static CharRange KoreanRange = new('가', '힣');
    public static CharRange NumberRange = new('0', '9');

    public static char MapByMin(char value, CharRange from, CharRange to)
    {
        return (char)(value - from.min + to.min);
    }
    public static char MapByMax(char value, CharRange from, CharRange to)
    {
        return (char)(value - from.max + to.max);
    }

    public char min;
    public char max;

    public CharRange(char min, char max)
    {
        this.min = min;
        this.max = max;
    }
    public bool IsRangeOrSame(int value)
    {
        return value >= min && value <= max;
    }
    public bool IsRangeInside(int value)
    {
        return value > min && value < max;
    }
    public bool IsRange(char value)
    {
        return value >= min && value <= max;
    }
    public bool IsRangeInside(char value)
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