
using System.ComponentModel;

using Discord;

namespace ManagerBot.Utils.Extension.Discord;

public static class OverwriteExtension
{
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
