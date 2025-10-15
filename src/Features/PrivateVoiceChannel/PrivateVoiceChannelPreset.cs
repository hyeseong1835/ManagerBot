using System.Text;
using Discord;


public class PrivateVoiceChannelPreset
{
    public struct UserJoinPermission
    {
        public ulong userId;
        public int value;
    }

    public string presetName;
    public UserJoinPermission[] userJoinPermissions;

    public PrivateVoiceChannelPreset(string presetName, UserJoinPermission[] userJoinPermissions)
    {
        this.presetName = presetName;
        this.userJoinPermissions = userJoinPermissions;
    }

    public int GetUserJoinPermissionValue(ulong userId)
    {
        int min = 0;
        int max = userJoinPermissions.Length - 1;

        int mid;
        UserJoinPermission midPermission;

        while (min <= max)
        {
            mid = (min + max) >> 1;
            midPermission = userJoinPermissions[mid];
            if (userJoinPermissions[mid].userId == userId)
            {
                return userJoinPermissions[mid].value;
            }
            else if (userJoinPermissions[mid].userId < userId)
            {
                min = mid + 1;
            }
            else
            {
                max = mid - 1;
            }
        }

        return 0; // 기본값
    }

    public Embed GetEmbed()
    {
        EmbedBuilder embedBuilder = new EmbedBuilder()
            .WithTitle(presetName)
            .WithColor(Color.Blue);

        StringBuilder descriptionBuilder = new StringBuilder();
        for (int i = 0; i < userJoinPermissions.Length; i++)
        {
            UserJoinPermission permission = userJoinPermissions[i];
            descriptionBuilder.AppendLine($"<@{permission.userId}>: {permission.value}");
        }
        embedBuilder.WithFields(new EmbedFieldBuilder()
        {
            Name = "사용자 참여 권한",
            Value = descriptionBuilder.ToString()
        });

        return embedBuilder.Build();
    }
}