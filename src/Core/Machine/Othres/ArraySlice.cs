using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public struct ArraySlice<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Memory<T>(ArraySlice<T> slice)
    {
        return slice.Memory;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Span<T>(ArraySlice<T> slice)
    {
        return slice.Span;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ArraySlice<T>(T[] array)
    {
        return new ArraySlice<T>(array, 0, array.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator ArraySlice<T>(Memory<T> segment)
    {
        if (false == MemoryMarshal.TryGetArray(segment, out ArraySegment<T> arraySegment))
        {
            throw new ArgumentException("The provided ReadOnlyMemory<T> does not have an underlying array.");
        }

        return (ArraySlice<T>)arraySegment;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator ArraySlice<T>(ArraySegment<T> segment)
    {
        return new ArraySlice<T>(segment.Array!, segment.Offset, segment.Count);
    }


    public T[] array;
    public int offset;
    public int count;

    public ArraySlice(T[] array, int offset, int count)
    {
        this.array = array;
        this.offset = offset;
        this.count = count;
    }

    public T this[int index] => array[index + offset];
    public Memory<T> Memory => new Memory<T>(array, offset, count);
    public Span<T> Span => new Span<T>(array, offset, count);
}