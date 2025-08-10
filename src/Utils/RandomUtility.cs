
public static class RandomUtility
{
    public static List<int> GenerateUniqueRandomNumbers(Random rand, int N)
    {
        List<int> numbers = new List<int>();

        for (int i = 0; i <= N; i++)
        {
            numbers.Add(i);
        }
        numbers.Shuffle(rand);
        return numbers;
    }
    /// <summary>
    /// 리스트를 섞습니다. [Fisher-Yates shuffle algorithm]
    /// </summary>
    public static List<TElement> Shuffle<TElement>(this List<TElement> list, Random rand)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = rand.Next(0, i + 1);
            list.Swap(i, randomIndex);
        }
        return list;
    }

    /// <summary>
    /// 배열을 섞습니다. [Fisher-Yates shuffle algorithm]
    /// </summary>
    public static TElement[] Shuffle<TElement>(this TElement[] array, Random rand)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = rand.Next(0, i + 1);
            array.Swap(i, randomIndex);
        }
        return array;
    }
}