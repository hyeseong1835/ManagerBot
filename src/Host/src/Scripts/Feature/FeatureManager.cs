using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;

using ManagerBot.Core;

namespace ManagerBot.Features;

public static class FeatureSystem
{
    static HostContext? hostContext;

    static LoadedFeature[]? loadedFeatures;
    public static LoadedFeature[] LoadedFeature => loadedFeatures?? Array.Empty<LoadedFeature>();

    public static async ValueTask LoadFeatures()
    {
        // 로드된 기능이 있으면 종료
        if (FeatureSystem.loadedFeatures != null && FeatureSystem.loadedFeatures.Length > 0)
        {
            Debug.LogWarning("FeatureSystem", "기능을 로드하지 않습니다. : 이미 로드된 기능이 존재합니다.");
            return;
        }

        // 디렉토리 확인 => 없으면 생성 후 종료
        if (false == Directory.Exists(PathHelper.FeaturesDirectoryPath))
        {
            Directory.CreateDirectory(PathHelper.FeaturesDirectoryPath);
            return;
        }
        //

        hostContext ??= new HostContext(ManagerBotCore.client);
        List<LoadedFeature> loadedFeatures = new();

        // 기능 디렉토리 순회
        foreach (string featureDir in Directory.EnumerateDirectories(PathHelper.FeaturesDirectoryPath))
        {
            // Dll 찾기 => 없으면 로드하지 않음
            string? dllPath = Directory.EnumerateFiles(featureDir, "*.dll").FirstOrDefault();
            if (dllPath == null)
            {
                Debug.LogWarning("FeatureSystem", $"기능을 로드하지 않습니다. : DLL을 찾을 수 없습니다: {featureDir}");
                continue;
            }
            //

            // ALC 생성
            FeatureLoadContext alc = new(dllPath);

            // 어셈블리 로드 => 실패하면 언로드
            Assembly featureAssembly;
            try
            {
                featureAssembly = alc.LoadFromAssemblyPath(Path.GetFullPath(dllPath));
            }
            catch (Exception e)
            {
                Debug.LogWarning("FeatureSystem", $"기능을 로드하지 않습니다. : 어셈블리를 로드하지 못했습니다: {dllPath}\n{e.GetType().Name}: {e.Message}");
                alc.Unload();
                continue;
            }
            //

            Type[] types = featureAssembly.GetTypes();
            Type featureBaseType = typeof(IFeature);

            foreach (Type type in types)
            {
                if (type.IsAbstract) continue;
                if (type.IsSubclassOf(featureBaseType)) continue;

                IFeature? feature = (IFeature?)Activator.CreateInstance(type);
                if (feature == null)
                {
                    Debug.LogWarning("FeatureSystem", $"기능을 로드하지 않습니다. : 기능 인스턴스를 생성하지 못했습니다: {type.FullName}");
                    continue;
                }

                try
                {
                    feature.Info = new FeatureInfo(feature.GetType().Name);
                    feature.Logger = new FeatureLogger(feature);
                    feature.HostCtx = hostContext;
                    await feature.InitializeAsync();
                }
                catch
                {
                    // 초기화 실패 시 정리
                    try { await feature.DisposeAsync(); } catch { }
                    alc.Unload();
                }

                loadedFeatures.Add(new LoadedFeature(feature, alc));
            }
        }
        //
    }

    public static async ValueTask<bool> UnloadFeature(IFeature feature)
    {
        return false;
    }
    /*
        public static async ValueTask<bool> UnloadAsync(LoadedFeature feature)
        {
            if (loadedFeatures.Length == 0)
                return false;

            LoadedFeature[] newFeatures = new LoadedFeature[loadedFeatures.Length - 1];

            LoadedFeature loadedFeature;
            int readI = 0;
            while (readI < loadedFeatures.Length)
            {
                loadedFeature = loadedFeatures[readI++];
                if (loadedFeature == feature)
                {
                    await feature.instance.DisposeAsync();
                    feature.alc.Unload();

                    int writeI = readI;
                    while (readI < loadedFeatures.Length)
                    {
                        newFeatures[readI++] = loadedFeatures[writeI++];
                    }

                    loadedFeatures = newFeatures;
                    return true;
                }

                newFeatures[readI] = loadedFeature;
            }

            return false;
        }
        public static async ValueTask UnloadAll()
        {
            LoadedFeature loadedFeature;
            for (int i = loadedFeatures.Length - 1; i >= 0; i--)
            {
                loadedFeature = loadedFeatures[i];

                await loadedFeature.instance.DisposeAsync();
                loadedFeature.alc.Unload();
            }
        }
    */


}
