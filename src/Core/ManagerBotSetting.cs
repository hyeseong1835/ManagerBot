using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;

public class ManagerBotSetting
{
    public static async ValueTask<ManagerBotSetting> LoadAsync()
    {
        string filePath = PathHelper.SettingFilePath;

        if (false == File.Exists(filePath))
        {
            // 설정 파일이 존재하지 않으면 기본 설정을 생성
            await SaveAsync(new ManagerBotSetting());
            throw new FileNotFoundException("설정 파일이 존재하지 않습니다. ", filePath);
        }

        using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read);
        return (await JsonSerializer.DeserializeAsync<ManagerBotSetting>(fileStream, new JsonSerializerOptions { WriteIndented = true }))!;
    }
    public static async ValueTask SaveAsync(ManagerBotSetting setting)
    {
        string filePath = PathHelper.SettingFilePath;

        using FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(fileStream, setting, new JsonSerializerOptions { WriteIndented = true });
    }

    [JsonInclude]
    [JsonPropertyName("botToken")]
    public string BotToken { get; private init; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("guildId")]
    public ulong GuildId { get; private init; } = 0;
}
