using System.Text.Json.Serialization;

namespace ManagerBot.Core.Features.MinecraftServerFeature;

public class MinecraftServerInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "Minecraft Server";

    [JsonPropertyName("description")]
    public string? Description { get; set; } = null;

    [JsonPropertyName("path")]
    public required string Path { get; set; }

    [JsonPropertyName("port")]
    public int Port { get; set; } = 25565;

    [JsonPropertyName("version")]
    public required string Version { get; set; }

    [JsonPropertyName("address")]
    public required string Address { get; set; }
}