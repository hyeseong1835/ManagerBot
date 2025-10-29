
using System.ComponentModel;

using ManagerBot.Core;

namespace Discord;
public static class OverwriteUtility
{
    public readonly static Overwrite EveryoneDenyAllOverwrite = new Overwrite(
        ManagerBotCore.Guild!.EveryoneRole.Id,
        PermissionTarget.Role,
        OverwritePermissionsUtility.DenyAllPermissions
    );
    public static Overwrite ReadOnlyOverwrite(ulong roleId)
        => new Overwrite(
            roleId,
            PermissionTarget.Role,
            OverwritePermissionsUtility.ReadOnlyPermissions
        );
}
