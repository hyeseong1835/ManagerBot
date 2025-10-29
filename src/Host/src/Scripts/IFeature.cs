using System;

namespace ManagerBot.Features;

public interface IFeature
{

}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class FeatureManifestAttribute : Attribute
{
    public string name;

    public FeatureManifestAttribute(string name)
    {
        this.name = name;
    }
}
