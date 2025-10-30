using System;
using System.Threading.Tasks;

namespace ManagerBot.Features;

public interface IFeatureLogger
{
    void LogInfo(string message, string? detail = null);
    void LogWarning(string message, string? detail = null);
    void LogWarning(Exception e);
    void LogError(string message, string? detail = null);
    void LogError(Exception e);
}
