
namespace Discord.WebSocket;
public static class SocketGuildUserUtility
{
    public static bool IsAdministrator(this SocketGuildUser user)
    {
        if (user.Guild.OwnerId == user.Id) return true;
        
        foreach (var role in user.Roles) {
            if (role.Permissions.Administrator) return true;
        }
        return false;
    }
    public static bool HasRole(this SocketGuildUser user, SocketRole role)
        => user.HasRole(role.Id);
    public static bool HasRole(this SocketGuildUser user, ulong roleId)
    {
        foreach (var role in user.Roles) {
            if (role.Id == roleId) return true;
        }
        return false;
    }
}