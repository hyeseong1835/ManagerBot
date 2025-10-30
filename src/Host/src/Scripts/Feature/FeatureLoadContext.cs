using System;
using System.Reflection;
using System.Runtime.Loader;

namespace ManagerBot.Features;

public sealed class FeatureLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;
    public FeatureLoadContext(string mainAssemblyPath)
        : base(isCollectible: true)
    {
        _resolver = new(mainAssemblyPath);
    }
    protected override Assembly? Load(AssemblyName name)
    {
        string? path = _resolver.ResolveAssemblyToPath(name);
        if (path == null)
            return null;

        return LoadFromAssemblyPath(path);
    }

    protected override IntPtr LoadUnmanagedDll(string name)
    {
        string? path = _resolver.ResolveUnmanagedDllToPath(name);
        if (path == null)
            return IntPtr.Zero;

        return LoadUnmanagedDllFromPath(path);
    }
}
