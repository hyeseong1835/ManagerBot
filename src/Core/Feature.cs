using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;

using HS.Common.Reflection;

namespace ManagerBot.Core;

public class FeatureInfo
{
    public string name;
    public Type type;
    public Feature? instance;

    internal FeatureInfo(string name, Type type, Feature? instance = null)
    {
        this.name = name;
        this.type = type;
        this.instance = instance;
    }
}

public abstract class Feature
{
    protected class FeatureInfoAttribute : Attribute
    {
        public string name;

        public FeatureInfoAttribute(string name)
        {
            this.name = name;
        }
    }
    public struct FeatureLoadInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        public FeatureLoadInfo() { }
    }
    public class FeatureSetting
    {
        [JsonPropertyName("featureLoad")]
        public FeatureLoadInfo[] FeatureLoads { get; set; } = Array.Empty<FeatureLoadInfo>();

        public FeatureSetting() { }
    }

    public const int featureInitializePriority = 50;

    public static readonly string FeatureSettingPath = $"{PathHelper.FeaturesDirectoryPath}/FeatureSetting.json";
    static FeatureSetting? featureSetting;
    public static int FeatureCount => featureInfos.Length;

    public static readonly FeatureInfo[] featureInfos = new Func<FeatureInfo[]>(
        static () =>
        {
            IEnumerable<Type> featureTypes = TypeUtility.GetChildTypes<Feature>();
            FeatureInfo[] infos = new FeatureInfo[featureTypes.Count()];
            int i = 0;
            foreach (Type type in featureTypes)
            {
                FeatureInfoAttribute? attribute = type.GetCustomAttribute<FeatureInfoAttribute>();
                if (attribute == null)
                    throw new InvalidOperationException($"Feature '{type.FullName}'는 FeatureInfoAttribute를 가져야 합니다.");

                Feature? instance = Activator.CreateInstance(type) as Feature;
                if (instance == null)
                    throw new InvalidOperationException($"Feature '{type.FullName}'를 인스턴스화 할 수 없습니다.");

                infos[i++] = new FeatureInfo(attribute.name, type, instance);
            }
            return infos;
        }
    ).Invoke();

    static Feature[] loadedFeatures = Array.Empty<Feature>();
    public static IReadOnlyList<Feature> LoadedFeatures => loadedFeatures;
    public static int LoadedFeatureCount => loadedFeatures.Length;

    static async ValueTask<FeatureSetting> LoadFeatureSettingAsync()
    {
        string featureSettingPath = FeatureSettingPath;
        if (File.Exists(featureSettingPath) == false)
        {
            // 설정 파일이 없으면 기본 설정을 생성
            await SaveFeatureSettingAsync(new FeatureSetting());
            _ = Debug.LogErrorAsync("Feature", $"기능 설정 파일이 존재하지 않습니다. 기본 설정을 생성했습니다: {featureSettingPath}");

            // 10초 후 종료
            await Task.Delay(10000);
            Environment.Exit(10);
        }

        using FileStream fileStream = new(featureSettingPath, FileMode.Open, FileAccess.Read);
        {
            return await JsonSerializer.DeserializeAsync<FeatureSetting>(
                fileStream,
                JsonHelper.readableJsonSerializationOptions
            ) ?? throw new JsonException("Feature 설정을 읽을 수 없습니다.");
        }
    }

    static async ValueTask LoadFeaturesAsync()
    {
        Feature.featureSetting = await LoadFeatureSettingAsync().ConfigureAwait(false);

        if (featureSetting == null)
            throw new InvalidOperationException("기능 설정이 로드되지 않았습니다.");

        // Feature 인스턴스 로드
        Feature.loadedFeatures = new Feature[Feature.featureSetting.FeatureLoads.Length];
        int featureIndex = 0;
        for (int loadFeatureIndex = 0; loadFeatureIndex < Feature.featureSetting.FeatureLoads.Length; loadFeatureIndex++)
        {
            FeatureLoadInfo loadInfo = Feature.featureSetting.FeatureLoads[loadFeatureIndex];

            // 타입 찾기
            FeatureInfo? featureInfo = FindFeatureInfo(loadInfo.Name);
            if (featureInfo == null)
            {
                _ = Debug.LogErrorAsync("Feature", $"기능 '{loadInfo.Name}'의 정보를 찾을 수 없습니다. 로드하지 않습니다.");
                continue;
            }

            // 인스턴스화
            Feature? feature = (Feature?)Activator.CreateInstance(featureInfo.type);
            if (feature == null)
            {
                _ = Debug.LogErrorAsync("Feature", $"기능 '{loadInfo.Name}'을(를) 인스턴스화 할 수 없습니다. 로드하지 않습니다.");
                continue;
            }

            // 정보 설정
            featureInfo.instance = feature;
            feature.info = featureInfo;

            Feature.loadedFeatures[featureIndex++] = feature;

            _ = Debug.LogErrorAsync("Feature", $"기능 '{feature.Info.name}' 인스턴싱.");
        }

        int writeIndex = 0;

        // Feature 로드
        for (int i = 0; i < featureIndex; i++)
        {
            Feature feature = Feature.loadedFeatures[i];

            try
            {
                await feature.Load();
                feature.isLoadSuccess = true;

                Feature.loadedFeatures[writeIndex++] = feature;
                _ = Debug.LogErrorAsync("Feature", $"기능 '{feature.Info.name}' 로드.");
            }
            catch (Exception ex)
            {
                _ = Debug.LogErrorAsync("Feature", $"기능 '{feature.Info.name}'을(를) 로드하는 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        Array.Resize(ref Feature.loadedFeatures, writeIndex);
    }
    static async ValueTask UnloadFeaturesAsync()
    {
        for (int i = 0; i < loadedFeatures.Length; i++)
        {
            Feature feature = loadedFeatures[i];

            try
            {
                await feature.Unload();
            }
            catch (Exception ex)
            {
                _ = Debug.LogErrorAsync("Feature", $"기능 '{feature.Info.name}'을(를) 언로드하는 중 오류가 발생했습니다: {ex.Message}");
            }
        }
    }

    static FeatureInfo? FindFeatureInfo(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        for (int i = 0; i < Feature.featureInfos.Length; i++)
        {
            FeatureInfo featureInfo = Feature.featureInfos[i];
            if (featureInfo.name.Equals(name))
            {
                return featureInfo;
            }
        }
        return null;
    }

    static async ValueTask SaveFeatureSettingAsync(FeatureSetting setting)
    {
        using FileStream fileStream = new(FeatureSettingPath, FileMode.Create, FileAccess.Write);
        {
            await JsonSerializer.SerializeAsync(fileStream, setting, JsonHelper.readableJsonSerializationOptions).ConfigureAwait(false);
        }
    }


    #region 이벤트

    [OnInitializeMethod(featureInitializePriority)]
    static async ValueTask Initialize()
    {
        await LoadFeaturesAsync();
    }

    [OnStopMethod]
    static async ValueTask Stop()
    {
        await UnloadFeaturesAsync();

        // 기능 설정 저장
        if (featureSetting != null)
        {
            await SaveFeatureSettingAsync(featureSetting);
        }
    }

    #endregion

    bool isLoadSuccess;
    public bool IsLoadSuccss => isLoadSuccess;

    string? path;
    public string Path => (path ??= $"{PathHelper.FeaturesDirectoryPath}/{info!.name}");

    string? settingPath;
    public string SettingPath => (settingPath ??= $"{Path}/setting.json");

    string? dataPath;
    public string DataPath => (dataPath ??= $"{Path}/Data");

    FeatureInfo? info;
    public FeatureInfo Info => info!;

    public virtual string? DescriptionInManagerBotPanel => (isLoadSuccess)? $"{info?.name?? "null"}"
                                                                          : $"{info?.name?? "null"}: 로드 실패";

    public virtual ValueTask Load() => ValueTask.CompletedTask;
    public virtual ValueTask Unload() => ValueTask.CompletedTask;


    #region 설정 파일

    public ValueTask<TSetting> LoadSettingAsync<TSetting>()
        where TSetting : class, new()
    {
        string settingPath = SettingPath;

        // 설정 파일이 없으면 기본 설정을 생성
        if (false == File.Exists(settingPath))
        {
            _ = Debug.LogErrorAsync("Feature", $"설정 파일이 존재하지 않습니다. 기본 설정을 생성합니다: {settingPath}");

            Directory.CreateDirectory(Path);
            SaveSettingAsync<TSetting>().GetAwaiter().GetResult();
            throw new FileNotFoundException("설정 파일이 존재하지 않습니다.", settingPath);
        }

        // 설정 파일을 읽어오기
        return ReadSettingAsync<TSetting>();
    }
    async ValueTask<TSetting> ReadSettingAsync<TSetting>()
        where TSetting : class, new()
    {
        // 설정 파일을 읽어오기
        using FileStream fileStream = new(SettingPath, FileMode.Open, FileAccess.Read);
        return await JsonSerializer.DeserializeAsync<TSetting>(fileStream, JsonHelper.readableJsonSerializationOptions).ConfigureAwait(false)
            ?? throw LoadSettingFailException();
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
