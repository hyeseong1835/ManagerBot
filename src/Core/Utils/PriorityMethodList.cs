#pragma warning disable CS8600 // null 리터럴 또는 가능한 null 값을 null을 허용하지 않는 형식으로 변환하는 중입니다.
#pragma warning disable CS8602 // null 가능 참조에 대한 역참조입니다.

using System.Reflection;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public abstract class PriorityMethodAttribute : Attribute
{
    public int priority;

    public PriorityMethodAttribute(int priority = 100)
    {
        this.priority = priority;
    }
    public static async Task FindAndInvoke<TAttribute>()
        where TAttribute : PriorityMethodAttribute
        => await FindAndInvoke<TAttribute>(
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        );
    public static async Task FindAndInvoke<TAttribute>(IEnumerable<MethodInfo> methodInfos)
        where TAttribute : PriorityMethodAttribute
    {
        PriorityQueue<MethodInfo, int> queue = new();

        foreach (MethodInfo method in methodInfos)
        {
            TAttribute? attribute = method.GetCustomAttribute<TAttribute>();

            if (attribute == null) continue;

            queue.Enqueue(method, attribute.priority);
        }

        for (;;)
        {
            if (!queue.TryDequeue(out MethodInfo? methodInfo, out int priority)) break;

            if (methodInfo.ReturnType == typeof(Task))
            {
                await (Task)methodInfo.Invoke(null, null);
            }
            else if (methodInfo.ReturnType == typeof(ValueTask))
            {
                await ((ValueTask)methodInfo.Invoke(null, null)!);
            }
            else if (methodInfo.ReturnType == typeof(void))
            {
                methodInfo.Invoke(null, null);
            }
            else
            {
                Console.WriteLine($"[ ERROR ] {methodInfo.DeclaringType?.Name}/{methodInfo.Name}의 반환 형식({methodInfo.ReturnType})이 잘못되었습니다.");
            }
        }
    }
}
[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public abstract class PriorityMethodAttribute<TParam> : Attribute
{
    public int priority;

    public PriorityMethodAttribute(int priority = 100)
    {
        this.priority = priority;
    }
    public static async Task FindAndInvoke<TAttribute>(TParam param)
        where TAttribute : PriorityMethodAttribute<TParam>
        => await FindAndInvoke<TAttribute>(
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)),
            param
        );
    public static async Task FindAndInvoke<TAttribute>(IEnumerable<MethodInfo> methodInfos, TParam param)
        where TAttribute : PriorityMethodAttribute<TParam>
    {
        PriorityQueue<MethodInfo, int> queue = new();

        foreach (MethodInfo method in methodInfos)
        {
            TAttribute? attribute = method.GetCustomAttribute<TAttribute>();

            if (attribute == null) continue;

            queue.Enqueue(method, attribute.priority);
        }

        for (;;)
        {
            if (!queue.TryDequeue(out MethodInfo? methodInfo, out int priority)) break;

            if (methodInfo.ReturnType == typeof(Task))
            {
                await (Task)methodInfo.Invoke(null, [ param ]);
            }
            else if (methodInfo.ReturnType == typeof(ValueTask))
            {
                await ((ValueTask)methodInfo.Invoke(null, [ param ])!);
            }
            else if (methodInfo.ReturnType == typeof(void))
            {
                methodInfo.Invoke(null, [ param ]);
            }
            else
            {
                Console.WriteLine($"[ ERROR ] {methodInfo.DeclaringType?.Name}/{methodInfo.Name}의 반환 형식({methodInfo.ReturnType})이 잘못되었습니다.");
            }
        }
    }
}
[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public abstract class PriorityMethodAttribute<TParam1, TParam2> : Attribute
{
    public int priority;

    public PriorityMethodAttribute(int priority = 100)
    {
        this.priority = priority;
    }
    public static async Task FindAndInvoke<TAttribute>(TParam1 param1, TParam2 param2)
        where TAttribute : PriorityMethodAttribute
        => await FindAndInvoke<TAttribute>(
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)),
            param1, param2
        );
    public static async Task FindAndInvoke<TAttribute>(IEnumerable<MethodInfo> methodInfos, TParam1 param1, TParam2 param2)
        where TAttribute : PriorityMethodAttribute
    {
        PriorityQueue<MethodInfo, int> queue = new();

        foreach (MethodInfo method in methodInfos)
        {
            TAttribute? attribute = method.GetCustomAttribute<TAttribute>();

            if (attribute == null) continue;

            queue.Enqueue(method, attribute.priority);
        }

        for (;;)
        {
            if (!queue.TryDequeue(out MethodInfo? methodInfo, out int priority)) break;

            if (methodInfo.ReturnType == typeof(Task))
            {
                await (Task)methodInfo.Invoke(null, [ param1, param2 ]);
            }
            else if (methodInfo.ReturnType == typeof(ValueTask))
            {
                await ((ValueTask)methodInfo.Invoke(null, [ param1, param2 ])!);
            }
            else if (methodInfo.ReturnType == typeof(void))
            {
                methodInfo.Invoke(null, [ param1, param2 ]);
            }
            else
            {
                Console.WriteLine($"[ ERROR ] {methodInfo.DeclaringType?.Name}/{methodInfo.Name}의 반환 형식({methodInfo.ReturnType})이 잘못되었습니다.");
            }
        }
    }
}

public abstract class PriorityMethodQueue
{
    public static void BulkFind(params PriorityMethodQueue[] attributes)
        => BulkFind(
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)),
            attributes
        );

    public static void BulkFind(IEnumerable<MethodInfo> methodInfos, params PriorityMethodQueue[] attributes)
    {
        foreach (MethodInfo method in methodInfos)
        {
            PriorityMethodAttribute? attribute = method.GetCustomAttribute<PriorityMethodAttribute>();

            if (attribute == null) continue;

            for (int i = 0; i < attributes.Length; i++)
            {
                PriorityMethodQueue queue = attributes[i];
                if (attribute.GetType() != queue.attributeType) continue;

                if (queue.queue == null) queue.queue = new();
                queue.queue.Enqueue(method, attribute.priority);
                break;
            }
        }
    }

    public abstract Type attributeType { get; }
    public PriorityQueue<MethodInfo, int>? queue;

    public abstract void Find(IEnumerable<MethodInfo> methodInfos);
    public void Find() => Find(
        AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
    );
    public async Task Invoke()
    {
        for (; ; )
        {
            if (!queue.TryDequeue(out MethodInfo? methodInfo, out int priority)) break;

            if (methodInfo.ReturnType == typeof(Task))
            {
                await (Task)methodInfo.Invoke(null, null);
            }
            else if (methodInfo.ReturnType == typeof(void))
            {
                methodInfo.Invoke(null, null);
            }
            else
            {
                Console.WriteLine($"[ ERROR ] {methodInfo.DeclaringType?.Name}/{methodInfo.Name}의 반환 형식({methodInfo.ReturnType})이 잘못되었습니다.");
            }
        }
    }
    public async Task FindAndInvoke()
    {
        Find();
        await Invoke();
    }
}
public class PriorityMethodQueue<TAttribute> : PriorityMethodQueue
    where TAttribute : PriorityMethodAttribute
{
    public override Type attributeType => typeof(TAttribute);

    public override void Find(IEnumerable<MethodInfo> methodInfos)
    {
        if (queue == null) queue = new();

        foreach (MethodInfo method in methodInfos)
        {
            TAttribute? attribute = method.GetCustomAttribute<TAttribute>();

            if (attribute == null) continue;

            queue.Enqueue(method, attribute.priority);
        }
    }
}