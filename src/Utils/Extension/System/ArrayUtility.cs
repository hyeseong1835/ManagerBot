
namespace System;
public static class ArrayUtility
{
    public static void Swap<TElement>(this TElement[] array, int index1, int index2)
    {
        TElement temp = array[index1];
        array[index1] = array[index2];
        array[index2] = temp;
    }
}