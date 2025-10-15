using System;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;

namespace ManagerBot.Utils.PriorityMethod;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public abstract class PriorityMethodAttribute : Attribute
{
    public int priority;

    public PriorityMethodAttribute(int priority)
    {
        this.priority = priority;
    }
    public static async Task FindAndInvoke<TAttribute>()
        where TAttribute : PriorityMethodAttribute
        => await FindAndInvoke<TAttribute>(AppDomain.CurrentDomain.GetAssemblies());

    public static async Task FindAndInvoke<TAttribute>(Assembly[] assemblies)
        where TAttribute : PriorityMethodAttribute
        => await FindAndInvoke<TAttribute>(
            assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        );

    public static async Task FindAndInvoke<TAttribute>(IEnumerable<Type> types)
        where TAttribute : PriorityMethodAttribute
        => await FindAndInvoke<TAttribute>(types);

    public static async Task FindAndInvoke<TAttribute>(IEnumerable<MethodInfo> methodInfos)
        where TAttribute : PriorityMethodAttribute
    {
        PriorityQueue<PriorityMethodInfo, int> queue = new();

        foreach (MethodInfo method in methodInfos)
        {
            TAttribute? attribute = method.GetCustomAttribute<TAttribute>();

            if (attribute == null) continue;

            queue.Enqueue(new(method, attribute), attribute.priority);
        }

        while (queue.TryDequeue(out PriorityMethodInfo methodInfo, out int priority))
        {
            await (ValueTask)methodInfo.info.Invoke(null, null)!;
        }
    }
}

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public abstract class PriorityMethodAttribute<TParam> : Attribute
{
    public int priority;
    public PriorityMethodOption option;

    public PriorityMethodAttribute(int priority = 100, PriorityMethodOption option = PriorityMethodOption.Auto)
    {
        this.priority = priority;
        this.option = option;
    }

    public static async Task FindAndInvoke<TAttribute>(TParam param)
        where TAttribute : PriorityMethodAttribute<TParam>
        => await FindAndInvoke<TAttribute>(
            param,
            AppDomain.CurrentDomain.GetAssemblies()
        );

    public static async Task FindAndInvoke<TAttribute>(TParam param, Assembly[] assemblies)
        where TAttribute : PriorityMethodAttribute<TParam>
        => await FindAndInvoke<TAttribute>(
            param,
            assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        );

    public static async Task FindAndInvoke<TAttribute>(TParam param, IEnumerable<Type> types)
        where TAttribute : PriorityMethodAttribute<TParam>
        => await FindAndInvoke<TAttribute>(param, types);

    public static async Task FindAndInvoke<TAttribute>(TParam param, IEnumerable<MethodInfo> methodInfos)
        where TAttribute : PriorityMethodAttribute<TParam>
    {
        PriorityQueue<PriorityMethodInfo<TParam>, int> queue = new();

        foreach (MethodInfo method in methodInfos)
        {
            TAttribute? attribute = method.GetCustomAttribute<TAttribute>();

            if (attribute == null) continue;

            queue.Enqueue(new(method, attribute), attribute.priority);
        }

        if (queue.Count == 0)
            return;

        object?[]? paramArray = [ param ];

        while (queue.TryDequeue(out PriorityMethodInfo<TParam> methodInfo, out int priority))
        {
            switch (methodInfo.attribute.option)
            {
                case PriorityMethodOption.Auto:
                {
                    switch (methodInfo.info.ReturnType)
                    {
                        case Type t when t == typeof(ValueTask):
                        {
                            await (ValueTask)methodInfo.info.Invoke(null, paramArray)!;
                            break;
                        }

                        case Type t when t == typeof(Task):
                        {
                            await (Task)methodInfo.info.Invoke(null, paramArray)!;
                            break;
                        }

                        case Type t when typeof(Task).IsAssignableFrom(t):
                        {
                            await (Task)methodInfo.info.Invoke(null, paramArray)!;
                            break;
                        }

                        default:
                        {
                            methodInfo.info.Invoke(null, paramArray);
                            break;
                        }
                    }
                    break;
                }

                case PriorityMethodOption.NonAwait:
                {
                    methodInfo.info.Invoke(null, paramArray);
                    break;
                }

                case PriorityMethodOption.Await:
                {
                    switch (methodInfo.info.ReturnType)
                    {
                        case Type t when t == typeof(ValueTask):
                        {
                            await (ValueTask)methodInfo.info.Invoke(null, paramArray)!;
                            break;
                        }

                        case Type t when t == typeof(Task):
                        {
                            await (Task)methodInfo.info.Invoke(null, paramArray)!;
                            break;
                        }

                        case Type t when typeof(Task).IsAssignableFrom(t):
                        {
                            await (Task)methodInfo.info.Invoke(null, paramArray)!;
                            break;
                        }

                        default:
                            throw new InvalidOperationException($"Method {methodInfo.info.Name}의 반환 형식은 대기할 수 없습니다.");
                    }
                    break;
                }
            }
        }
    }
}
