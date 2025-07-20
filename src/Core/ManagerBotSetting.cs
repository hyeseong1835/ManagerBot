using System.Text.Json;
using System.Text.Json.Serialization;

public class ManagerBotSetting
{
    public static ManagerBotSetting Load(string filePath)
    {
        if (false == File.Exists(filePath))
            return new ManagerBotSetting();

        using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read);

        return JsonSerializer.Deserialize<ManagerBotSetting>(fileStream)!;
    }

    [JsonInclude]
    [JsonPropertyName("BotToken")]
    string? _BotToken { get => botToken; init => botToken = value?? throw new ArgumentNullException(nameof(value), "BotToken은 null일 수 없습니다."); }
    string? botToken;
    public string BotToken => botToken?? throw new InvalidOperationException("BotToken이 설정되지 않았습니다.");


    public ulong GuildId { get; private init; }
}