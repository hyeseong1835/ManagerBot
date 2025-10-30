namespace ManagerBot.Features;

public class FeatureInfo : IFeatureInfo
{
    public string name;
    public string Name => name;

    public FeatureInfo(string name)
    {
        this.name = name;
    }
}
