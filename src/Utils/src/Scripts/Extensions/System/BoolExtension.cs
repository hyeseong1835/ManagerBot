using System;

using Discord;

namespace ManagerBot.Utils.Extension.System;

public static class BoolExtension
{
    public static string ToEmojiString(this bool value)
    {
        return value? ":white_check_mark:" : ":x:";
    }
}
