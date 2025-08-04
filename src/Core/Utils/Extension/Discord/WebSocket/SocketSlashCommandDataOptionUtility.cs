
namespace Discord.WebSocket;
public static class SocketSlashCommandDataOptionUtility
{
    public static TValue? GetValue<TValue>(this SocketSlashCommandDataOption data, string name)
        where TValue : class
    {
        SocketSlashCommandDataOption? option = data.Options.FirstOrDefault(x => x.Name == name);
        if (option == null) return null;
        
        return option.Value as TValue;
    }
    public static bool TryGetValue<TValue>(this SocketSlashCommandDataOption data, string name, out TValue? value)
    {
        SocketSlashCommandDataOption? option = data.Options.FirstOrDefault(x => x.Name == name);
        if (option == null) {
            value = default;
            return false;
        }
        value = (TValue)option.Value;
        return true;
    }
}