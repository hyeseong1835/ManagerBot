using HS.Common.System;

namespace ManagerBot.Core;

public abstract class Feature
{
    public static Feature[] features = TypeUtility.CreateChildInstances<Feature>();

    public abstract string Name { get; }

    public abstract ValueTask Load();
}