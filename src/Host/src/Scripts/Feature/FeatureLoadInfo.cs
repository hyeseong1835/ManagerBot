using System.Text.Json.Serialization;

namespace ManagerBot.Features;

public struct FeatureLoadInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    public FeatureLoadInfo()
    {

    }
}
