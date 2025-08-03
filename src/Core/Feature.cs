using System.Text.Json;
using HS.Common.System;

namespace ManagerBot.Core;

public abstract class Feature
{
    public static Feature[] features = TypeUtility.CreateChildInstances<Feature>();

    [OnBotInitializeMethod]
    public static async ValueTask Initialize()
    {
        for (int i = 0; i < features.Length; i++)
        {
            await features[i].Load();
        }
    }

    [OnBotStopMethod]
    public static async ValueTask Stop()
    {
        for (int i = 0; i < features.Length; i++)
        {
            await features[i].Unload();
        }
    }


    string? path;
    public string Path => (path ??= $"{PathHelper.featuresDirectoryPath}/{Name}");

    string? dataPath;
    public string DataPath => (dataPath ??= $"{Path}/Data");

    public abstract string Name { get; }

    public virtual ValueTask Load() => ValueTask.CompletedTask;
    public virtual ValueTask Unload() => ValueTask.CompletedTask;


    #region 설정 파일


    string? settingPath;
    public string SettingPath => (settingPath ??= $"{Path}/setting.json");

    public ValueTask<TSetting?> LoadSettingAsync<TSetting>()
        where TSetting : class, new()
    {
        string settingPath = SettingPath;

        // 설정 파일이 없으면 기본 설정을 생성
        if (false == File.Exists(settingPath))
        {
            Directory.CreateDirectory(Path);
            SaveSettingAsync<TSetting>().GetAwaiter().GetResult();
            throw new FileNotFoundException("설정 파일이 존재하지 않습니다.", settingPath);
        }

        // 설정 파일을 읽어오기
        using FileStream fileStream = new(settingPath, FileMode.Open, FileAccess.Read);
        return JsonSerializer.DeserializeAsync<TSetting>(fileStream, JsonHelper.readableJsonSerializationOptions);
    }
    public Task SaveSettingAsync<TSetting>(TSetting? setting = null)
        where TSetting : class, new()
    {
        if (setting == null)
        {
            if (File.Exists(SettingPath))
                return Task.CompletedTask;

            setting = new TSetting();
        }
        using FileStream fileStream = new(SettingPath, FileMode.Create, FileAccess.Write);
        return JsonSerializer.SerializeAsync(fileStream, setting, JsonHelper.readableJsonSerializationOptions);
    }

    public Exception LoadSettingFailException()
        => new JsonException($"설정 파일을 읽을 수 없습니다. '{SettingPath}'");

    #endregion
}