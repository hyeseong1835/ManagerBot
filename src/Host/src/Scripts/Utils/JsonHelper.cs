using System.Text.Json;
using System.Text.Json.Serialization;

internal class JsonHelper
{
    public static readonly JsonSerializerOptions readableJsonSerializationOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip
    };
}
