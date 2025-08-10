public static class PathHelper
{
    public static string DataDirectoryPath { get; private set; } = string.Empty;
    public static string SettingFilePath { get; private set; } = string.Empty;

    public static string FeaturesDirectoryPath { get; private set; } = string.Empty;
    public static string GetFeaturePath(string featureName) => $"{FeaturesDirectoryPath}/{featureName}";

    public static void SetDataDirectoryPath(string path)
    {
        DataDirectoryPath = path;
        SettingFilePath = $"{DataDirectoryPath}/setting.json";
        FeaturesDirectoryPath = $"{DataDirectoryPath}/Features";
    }
}