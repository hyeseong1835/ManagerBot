using System.Text.Json.Serialization;

namespace ManagerBot.Core.Features.RaspberrypiFeature;

public class RaspberrypiSetting
{
    [JsonPropertyName("dashboardChannelId")]
    public ulong DashboardChannelId { get; set; } = 0;
}
[FeatureInfo("Raspberrypi")]
public class Feature_Raspberrypi : Feature
{

}