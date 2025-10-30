using ManagerBot.Features;

public class LoadedFeature
{
    public IFeature instance;
    public FeatureLoadContext alc;

    public LoadedFeature(IFeature instance, FeatureLoadContext alc)
    {
        this.instance = instance;
        this.alc = alc;
    }
}
