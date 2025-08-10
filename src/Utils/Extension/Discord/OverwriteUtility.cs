
using System.ComponentModel;
using ManagerBot.Core;

namespace Discord;
public static class OverwriteUtility
{
    public readonly static Overwrite EveryoneDenyAllOverwrite = new Overwrite(
        ManagerBotCore.Guild.EveryoneRole.Id,
        PermissionTarget.Role,
        OverwritePermissionsUtility.DenyAllPermissions
    );
    public static Overwrite ReadOnlyOverwrite(ulong roleId)
        => new Overwrite(
            roleId,
            PermissionTarget.Role,
            OverwritePermissionsUtility.ReadOnlyPermissions
        );
    public static string TargetMention(this Overwrite overwrite)
    {
        switch (overwrite.TargetType)
        {
            case PermissionTarget.Role:
                return $"<@&{overwrite.TargetId}>";
            case PermissionTarget.User:
                return $"<@{overwrite.TargetId}>";
            default:
                throw new InvalidEnumArgumentException("Unknown target type.");
        }
    }
}