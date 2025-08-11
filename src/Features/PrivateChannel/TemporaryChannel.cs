using Discord;
using Discord.WebSocket;
using ManagerBot.Features.PrivateChannelFeature;
using ManagerBot.Utils.AutoDeleteVoiceChannelUtils;

public class TemporaryChannel
{
    public static OverwritePermissions userPermissions = new OverwritePermissions(

        // 기본 권한
        viewChannel: PermValue.Allow,
        connect: PermValue.Allow,
        speak: PermValue.Allow,
        stream: PermValue.Allow,

        // 사운드보드
        useSoundboard: PermValue.Allow,
        useExternalSounds: PermValue.Allow,
        startEmbeddedActivities: PermValue.Allow,
        setVoiceChannelStatus: PermValue.Allow,

        // 메시지 채널
        readMessageHistory: PermValue.Allow,

        createInstantInvite: PermValue.Deny,
        manageChannel: PermValue.Deny,
        manageMessages: PermValue.Deny,
        embedLinks: PermValue.Deny,
        attachFiles: PermValue.Deny,
        mentionEveryone: PermValue.Deny,
        useExternalEmojis: PermValue.Deny,
        muteMembers: PermValue.Deny,
        deafenMembers: PermValue.Deny,
        moveMembers: PermValue.Deny,
        useVoiceActivation: PermValue.Deny,
        manageRoles: PermValue.Deny,
        manageWebhooks: PermValue.Deny,
        prioritySpeaker: PermValue.Deny,
        requestToSpeak: PermValue.Deny,
        usePublicThreads: PermValue.Deny,
        usePrivateThreads: PermValue.Deny,
        useExternalStickers: PermValue.Deny,
        sendMessagesInThreads: PermValue.Deny,
        createEvents: PermValue.Deny,
        sendVoiceMessages: PermValue.Deny,
        useClydeAI: PermValue.Deny,
        sendPolls: PermValue.Deny
    );

    public static async Task<TemporaryChannel> Create(string name,
        ThreadArchiveDuration autoArchiveDuration = ThreadArchiveDuration.OneDay, bool invitable = true)
    {
        TemporaryChannel channel = new TemporaryChannel(
            await Feature_TemporaryChannel.ParentChannel.CreateThreadAsync(
                name,
                ThreadType.PrivateThread,
                autoArchiveDuration,
                null,
                invitable,
                null,
                null
            )
        );
        Feature_TemporaryChannel.activeTemporaryChannels.Add(channel);
        return channel;
    }

    public readonly SocketThreadChannel thread;
    public SocketVoiceChannel? voiceChannel;

    TemporaryChannel(SocketThreadChannel thread, SocketVoiceChannel? voiceChannel = null)
    {
        this.thread = thread;
        this.voiceChannel = voiceChannel;
    }

    public Task DeleteVoiceChannel()
    {
        if (voiceChannel == null)
            return Task.CompletedTask;

        AutoDeleteVoiceChannel.Unregist(voiceChannel);
        return voiceChannel.DeleteAsync();
    }

    public Task SyncVoiceChannelUserToThread()
    {
        if (voiceChannel == null)
            return Task.CompletedTask;

        IEnumerable<SocketThreadUser> threadUsers = thread.Users.Where(u => u.IsBot == false);
        int userCount = threadUsers.Count();

        Overwrite[] permissions = new Overwrite[userCount + 1];
        int permissionIndex = 0;

        permissions[permissionIndex++] = OverwriteUtility.EveryoneDenyAllOverwrite;

        foreach (SocketThreadUser user in threadUsers)
        {
            permissions[permissionIndex++] = new Overwrite(
                user.Id,
                PermissionTarget.User,
                userPermissions
            );
        }

        return voiceChannel.ModifyAsync(
            (properties) =>
            {
                properties.PermissionOverwrites = permissions;
                properties.UserLimit = userCount;
            }
        );
    }
}