#pragma warning disable CS8600 // null 리터럴 또는 가능한 null 값을 null을 허용하지 않는 형식으로 변환하는 중입니다.
#pragma warning disable CS8603 // 가능한 null 참조 반환입니다.

using System.Runtime.CompilerServices;

namespace System;
public class TupleUtility
{
    public static ITuple CreateTuple(object[] values, params Type[] genericTypes)
    {
        Type emptyTupleType;
        switch (genericTypes.Length)
        {
            case 1: emptyTupleType = typeof(Tuple<>); break;
            case 2: emptyTupleType = typeof(Tuple<,>); break;
            case 3: emptyTupleType = typeof(Tuple<,,>); break;
            case 4: emptyTupleType = typeof(Tuple<,,,>); break;
            case 5: emptyTupleType = typeof(Tuple<,,,,>); break;
            case 6: emptyTupleType = typeof(Tuple<,,,,,>); break;
            case 7: emptyTupleType = typeof(Tuple<,,,,,,>); break;
            case 8: emptyTupleType = typeof(Tuple<,,,,,,,>); break;
            default: throw new NotSupportedException($"Unsupported tuple length: {genericTypes.Length}");
        }
        return (ITuple)Activator.CreateInstance(emptyTupleType.MakeGenericType(genericTypes), values);
    }
}
