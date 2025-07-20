using HS.Common.System;

namespace ManagerBot.Core;

public abstract class Feature
{
    public static Feature[] features = TypeUtility.CreateChildInstances<Feature>();
    public static async ValueTask Initialize()
    {
        for (int i = 0; i < features.Length; i++)
        {
            await features[i].Load();
        }
    }


    string? path;
    public string Path => path ??= $"{PathHelper.featuresDirectoryPath}/{Name}";
    public string DataPath => $"{Path}/Data";

    public abstract string Name { get; }

    public abstract ValueTask Load();
    public abstract ValueTask Unload();
}