using System;
using ManagerBot.Core;

namespace ManagerBot.Features;

public class FeatureLogger : IFeatureLogger
{
    public IFeature feature;

    public FeatureLogger(IFeature feature)
    {
        this.feature = feature;
    }

    public void LogInfo(string message, string? detail = null)
        => Debug.Log(feature.Info.Name, message, detail);

    public void LogWarning(string message, string? detail = null)
        => Debug.LogWarning(feature.Info.Name, message);
    public void LogWarning(Exception e)
        => Debug.LogWarning(feature.Info.Name, e);

    public void LogError(string message, string? detail = null)
        => Debug.LogError(feature.Info.Name, message);
    public void LogError(Exception e)
        => Debug.LogError(feature.Info.Name, e);
}
