using Discord;
using Discord.WebSocket;

using ManagerBot.Utils.AutoDeleteVoiceChannelUtils;
using ManagerBot.Utils.DiscordHelper;
using ManagerBot.Features.PrivateChannelFeature;

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
            ),
            await ChannelHelper.CreateVoiceChannel(
                name,
                (property) =>
                {
                    property.Name = name;
                    property.PermissionOverwrites = OverwriteUtility.EveryoneDenyAllOverwriteArray;
                    property.UserLimit = 0;
                    property.CategoryId = Feature_TemporaryChannel.Setting.CategoryId;
                },
                null
            ),
            permissionOverwriteBufferGetter
        );
        Feature_TemporaryChannel.activeTemporaryChannels.Add(channel);

        await
        return channel;
    }
    public static async Task<TemporaryChannel> Create(string name,
        ThreadArchiveDuration autoArchiveDuration = ThreadArchiveDuration.OneDay, bool invitable = true)
        => Create(name,
            new TempArrayPooler<Overwrite>(),
            autoArchiveDuration,
            invitable
        );

    public readonly SocketThreadChannel thread;
    public readonly SocketVoiceChannel voiceChannel;

    public readonly ITempMemoryGetter<Overwrite> permissionOverwriteBufferGetter;

    TemporaryChannel(SocketThreadChannel thread, SocketVoiceChannel voiceChannel, ITempMemoryGetter<Overwrite> permissionOverwriteBufferGetter)
    {
        this.thread = thread;
        this.voiceChannel = voiceChannel;
        this.permissionOverwriteBufferGetter = permissionOverwriteBufferGetter;
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
