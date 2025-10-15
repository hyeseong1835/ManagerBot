using System.Text.Json;
using System.Text.Json.Serialization;

namespace ManagerBot.Core.Features.GroupFeature;

public class Feature_Group : Feature
{
    #region Static

    public static readonly string path = PathHelper.GetFeaturePath("Group");
    public static readonly string groupDataPath = $"{path}/Data/groups.json";

    static Feature_Group? instance;
    public static Feature_Group Instance => instance!;


    public static async ValueTask SaveGroupData()
    {
        // 로드되지 않은 경우 저장하지 않음
        if (instance == null || instance.groups == null)
            return;

        try
        {
            GroupData[] groupDataArray = new GroupData[instance.groups.Length];
            for (int i = 0; i < instance.groups.Length; i++)
            {
                groupDataArray[i] = instance.groups[i].groupData;
            }

            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(
                groupDataArray,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.Never
                }
            );
            await File.WriteAllBytesAsync(groupDataPath, bytes);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"그룹 데이터를 저장하지 못했습니다.\n{ex.Message}");
        }
    }

    #endregion


    #region Instance

    public override string Name => "Group";

    public Group[]? groups;

    public Feature_Group()
    {
        instance = this;
    }

    public override async ValueTask Load()
    {
        if (false == Directory.Exists(path))
            Directory.CreateDirectory(path);

        await Load_GroupData();
    }
    async ValueTask Load_GroupData()
    {
        if (!File.Exists(groupDataPath))
        {
            groups = Array.Empty<Group>();
            return;
        }

        using FileStream stream = new(groupDataPath, FileMode.Open, FileAccess.Read);
        GroupData[]? groupDataArray = await JsonSerializer.DeserializeAsync<GroupData[]>(stream);
        if (groupDataArray == null)
        {
            groups = Array.Empty<Group>();
            return;
        }
        groups = new Group[groupDataArray.Length];
        for (int i = 0; i < groupDataArray.Length; i++)
        {
            groups[i] = new Group(groupDataArray[i]);
        }
    }

    public override async ValueTask Unload()
    {
        await SaveGroupData();
    }

    #endregion
}