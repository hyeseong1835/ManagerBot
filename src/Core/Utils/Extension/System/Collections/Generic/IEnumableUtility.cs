
namespace System.Collections.Generic;
public static class IEnumableUtility
{
    /// <summary>
    /// IEnumerable를 순서가 뒤바뀐 스택으로 변환합니다.
    /// </summary>
    public static Stack<TElement> ToReversedStack<TElement>(this IEnumerable<TElement> collection)
    {
        Stack<TElement> stack = new Stack<TElement>();
        foreach (TElement element in collection)
        {
            stack.Push(element);
        }
        return stack;
    }
    
    public static IEnumerable<TElement> Clamp<TElement>(this IEnumerable<TElement> collection, int count)
    {
        if (collection.Count() > count)
        {
            return collection.Take(count);
        }
        else return collection;
    }
    public static IEnumerable<TElement> ClampLast<TElement>(this IEnumerable<TElement> collection, int count)
    {
        if (collection.Count() > count)
        {
            return collection.TakeLast(count);
        }
        else return collection;
    }

    public static IEnumerable<TElement> GetRange<TElement>(this IEnumerable<TElement> collection, int start, int end)
    {
        return collection.Skip(start).Take(end - start + 1);
    }
}