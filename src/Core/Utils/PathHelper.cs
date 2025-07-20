public static class PathHelper
{
    public readonly static string programDirectoryPath = "C:/Users/comet/Documents/ManagerBot";
    public readonly static string settingFilePath = $"{programDirectoryPath}/settings.json";

    public readonly static string featuresDirectoryPath = $"{programDirectoryPath}/Features";
    public static string GetFeaturePath(string featureName) => $"{featuresDirectoryPath}/{featureName}";
}