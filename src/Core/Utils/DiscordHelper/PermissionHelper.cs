using Discord;

namespace ManagerBot.Core.Utils.DiscordHelper;

public static class PermissionHelper
{
    public static readonly OverwritePermissions allowAllPermissions = new OverwritePermissions(
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow,
        PermValue.Allow
    );
    public static readonly OverwritePermissions denyAllPermissions = new OverwritePermissions(
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny,
        PermValue.Deny
    );


    static bool isReadySingleOverwriteBuffer = false;
    static Overwrite[] singleOverwriteBuffer = new Overwrite[1];
    public static Overwrite[] RentSingleOverwriteBuffer()
    {
        if (isReadySingleOverwriteBuffer == false)
            return new Overwrite[1];

        return singleOverwriteBuffer;
    }
    public static void ReturnSingleOverwriteBuffer(Overwrite[] buffer)
    {
        if (isReadySingleOverwriteBuffer)
            return;

        if (buffer.Length != 1)
            throw new ArgumentException("단일 오버라이트 버퍼는 반드시 길이가 1이어야 합니다.", nameof(buffer));

        singleOverwriteBuffer = buffer;
        isReadySingleOverwriteBuffer = true;
    }
}