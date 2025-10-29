using System;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;

namespace ManagerBot.Utils.PriorityMethod;

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
                queue.queue.Enqueue(new (method, attribute), attribute.priority);
                break;
            }
        }
    }

    public abstract Type attributeType { get; }
    public required PriorityQueue<PriorityMethodInfo, int> queue;

    public abstract void Find(IEnumerable<MethodInfo> methodInfos);
    public void Find() => Find(
        AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
    );
    public async Task Invoke()
    {
        while (queue.TryDequeue(out PriorityMethodInfo methodInfo, out int priority))
        {
            await (ValueTask)methodInfo.info.Invoke(null, null)!;
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

            queue.Enqueue(new (method, attribute), attribute.priority);
        }
    }
}
