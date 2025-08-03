public static class PathHelper
{
    public static string dataDirectoryPath = string.Empty;
    public static string settingFilePath = $"{dataDirectoryPath}/setting.json";

    public static string featuresDirectoryPath = $"{dataDirectoryPath}/Features";
    public static string GetFeaturePath(string featureName) => $"{featuresDirectoryPath}/{featureName}";
}