using System.Text.Json.Serialization;

namespace ManagerBot.Core.Features.MinecraftServerFeature;

public class MinecraftServerSetting
{
    [JsonPropertyName("dashboardChannelId")]
    public ulong DashboardChannelId { get; set; } = 0;
}

[FeatureInfo("MinecraftServer")]
public class Feature_MinecraftServer : Feature
{

}