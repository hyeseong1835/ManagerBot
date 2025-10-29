
using System.Linq;

using Discord.WebSocket;

namespace ManagerBot.Utils.Extension.Discord.WebSocket;

public static class SocketSlashCommandDataOptionExtension
{
    public static object? GetValue(this SocketSlashCommandDataOption data, string name)
    {
        SocketSlashCommandDataOption? option = data.Options.FirstOrDefault(x => x.Name == name);
        if (option == null) return null;

        return option.Value;
    }
    public static TValue? GetValue<TValue>(this SocketSlashCommandDataOption data, string name)
        where TValue : class
    {
        return data.GetValue(name) as TValue;
    }
}
