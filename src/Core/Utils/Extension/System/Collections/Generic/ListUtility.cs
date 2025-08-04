
namespace System.Collections.Generic;
public static class ListUtility
{
    public static void Swap<TElement>(this List<TElement> list, int index1, int index2)
    {
        TElement temp = list[index1];
        list[index1] = list[index2];
        list[index2] = temp;
    }
    public static void BulkAdd<TElement>(this List<TElement> list, params TElement[] elements)
    {
        list.AddRange(elements);
    }
}