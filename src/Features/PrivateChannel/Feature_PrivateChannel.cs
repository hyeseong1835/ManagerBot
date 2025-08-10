using System.Text.Json.Serialization;
using Discord.WebSocket;
using ManagerBot.Core.Utils.WaitingVoiceChannelUtils;

using ManagerBot.Core.Utils.DiscordHelper;
using Discord;
using ManagerBot.Utils.PriorityMethod;
using Discord.Rest;

namespace ManagerBot.Core.Features.PrivateChannelFeature;

public class PrivateChannelSetting
{
    [JsonPropertyName("categoryId")]
    public ulong CategoryId { get; set; } = 0;

    [JsonPropertyName("parentChannelId")]
    public ulong ParentChannelId { get; set; } = 0;

    [JsonPropertyName("waitingVoiceChannelId")]
    public ulong WaitingVoiceChannelId { get; set; } = 0;
}

[FeatureInfo("PrivateChannel")]
public class Feature_PrivateChannel : Feature
{
    // 싱글턴 인스턴스
    static Feature_PrivateChannel? instance;
    public static Feature_PrivateChannel Instance => instance!;

    // 설정
    PrivateChannelSetting? setting;
    public static PrivateChannelSetting Setting => instance!.setting!;

    // 스레드 부모 채널
    SocketGuildChannel? parentChannel;
    public SocketGuildChannel ParentChannel => parentChannel!;

    // 활성화된 임시 채널 목록
    List<SocketThreadChannel> activeTemporaryChannels = new List<SocketThreadChannel>();


    // 생성자
    public Feature_PrivateChannel()
    {
        instance = this;
    }

    public override async ValueTask Load()
    {
        // 설정 로드
        setting = await LoadSettingAsync<PrivateChannelSetting>()
            .ConfigureAwait(false);

        // 스레드 부모 채널 로드
        parentChannel = ChannelHelper.GetTextChannel(Feature_PrivateChannel.Setting.ParentChannelId);

        // 대기실 음성 채널 로드
        WaitingVoiceChannel waitingVoiceChannel = WaitingVoiceChannel.Add(setting.WaitingVoiceChannelId);
        waitingVoiceChannel.onPrivateChannelCreated += async (PrivateWaitingVoiceChannel privateChannel) =>
        {
            await privateChannel.voiceChannel.SendMessageAsync(
                $"비공개 음성 채널이 생성되었습니다."
            );
        };

        // 이벤트 구독
        ManagerBotCore.client.ThreadCreated += OnThreadCreated;
        ManagerBotCore.client.ThreadDeleted += OnThreadDeleted;
        ManagerBotCore.client.ThreadUpdated += OnThreadUpdated;

        ManagerBotCore.client.ThreadMemberJoined += OnThreadMemberJoined;
        ManagerBotCore.client.ThreadMemberLeft += OnThreadMemberLeft;
    }

    [OnBotInitializeMethod(Feature.featureInitializePriority + 1, PriorityMethodOption.NonAwait)]
    static async ValueTask AfterFeatureInitialize()
    {
        await UpdateManagedThreadVoiceChannel();
    }


    public static OverwritePermissions threadVoiceChannelEveryonePermissions =
        new OverwritePermissions(
            viewChannel: PermValue.Deny
        );
    public static OverwritePermissions threadVoiceChannelUserPermissions =
        new OverwritePermissions(
            viewChannel: PermValue.Allow
        );
    public static readonly ThreadArchiveDuration DefaultThreadArchiveDuration = ThreadArchiveDuration.OneDay;


    #region 이벤트

    static Task OnThreadCreated(SocketThreadChannel thread)
    {
        PrivateChannelSetting setting = Feature_PrivateChannel.Setting;

        // 임시 채널이 아니면 종료
        if (thread.ParentChannel.Id != Feature_PrivateChannel.Setting.ParentChannelId) return Task.CompletedTask;

        // 공개 스레드이면 제거
        if (thread.IsPrivateThread == false)
        {
            return thread.DeleteAsync();
        }

        return CreateVoiceChannelTo(thread).AsTask();
    }
    static Task OnThreadDeleted(Cacheable<SocketThreadChannel, ulong> thread)
    {
        PrivateChannelSetting setting = Feature_PrivateChannel.Setting;

        if (thread.HasValue == false) return Task.CompletedTask;
        if (thread.Value.ParentChannel.Id != Feature_PrivateChannel.Setting.ParentChannelId) return Task.CompletedTask;

        SocketVoiceChannel? voiceChannel = GetVoiceChannelByThreadName(thread.Value.Name);
        if (voiceChannel == null || voiceChannel.ConnectedUsers.Count > 0) return Task.CompletedTask;

        return DeleteVoiceChannel(thread.Value, voiceChannel);
    }

    static Task OnThreadUpdated(Cacheable<SocketThreadChannel, ulong> thread, SocketThreadChannel threadBefore)
    {
        PrivateChannelSetting setting = Feature_PrivateChannel.Setting;

        if (thread.HasValue == false) return Task.CompletedTask;
        if (thread.Value.ParentChannel.Id != Feature_PrivateChannel.Setting.ParentChannelId) return Task.CompletedTask;

        bool prevStateArchived = IsThreadArchived(threadBefore);
        bool curStateArchived = IsThreadArchived(thread.Value);

        // 스레드 상태가 변경되지 않았으면 종료
        if (prevStateArchived == curStateArchived)
            return Task.CompletedTask;

        // 스레드가 비활성화 되었으면 음성 채널 삭제
        if (curStateArchived)
        {
            return ArchiveThread(thread.Value);
        }
        // 스레드가 활성화 되었으면 음성 채널 생성
        else
        {
            return CreateVoiceChannelTo(thread.Value).AsTask();
        }
    }

    static async Task OnThreadMemberJoined(SocketThreadUser user)
    {
        if (user.Thread.ParentChannel.Id != Feature_PrivateChannel.Setting.ParentChannelId) return;

        ulong voiceChannelId = ExtractID(user.Thread.Name);
        if (voiceChannelId == 0) return;

        SocketVoiceChannel? voiceChannel = ManagerBotCore.Guild.GetVoiceChannel(voiceChannelId);
        if (voiceChannel == null)
        {
            await CreateVoiceChannelTo(user.Thread);
            return;
        }

        //음성 채널 권한 업데이트
        await AddVoiceChannelUser(voiceChannel, user);
    }
    static async Task OnThreadMemberLeft(SocketThreadUser user)
    {
        if (user.Thread.ParentChannel.Id != Feature_PrivateChannel.Setting.ParentChannelId) return;

        SocketVoiceChannel? voiceChannel = GetVoiceChannelByThreadName(user.Thread.Name);
        if (voiceChannel == null)
        {
            await CreateVoiceChannelTo(user.Thread);
            return;
        }

        //음성 채널 권한 업데이트
        await RemoveVoiceChannelUser(voiceChannel, user);
    }

    static async Task UpdateManagedThreadVoiceChannel()
    {
        SocketCategoryChannel threadsCategory = ManagerBotCore.Guild.GetCategoryChannel(Feature_PrivateChannel.Setting.CategoryId);
        SocketTextChannel temporaryChannel = ManagerBotCore.Guild.GetTextChannel(Feature_PrivateChannel.Setting.ParentChannelId);

        // 음성 채널 순회: 삭제 및 권한 업데이트
        foreach (SocketVoiceChannel voiceChannel in threadsCategory.Channels.OfType<SocketVoiceChannel>())
        {
            if (voiceChannel == null) continue;

            // 스레드가 없으면 삭제
            SocketThreadChannel? thread = GetThreadByVoiceChannelName(voiceChannel.Name);
            if (thread == null)
            {
                if (voiceChannel.ConnectedUsers.Count > 0) continue;

                await DeleteVoiceChannel(thread, voiceChannel);
                continue;
            }

            // 스레드가 비활성화 되었으면 삭제
            if (IsThreadArchived(thread))
            {
                if (voiceChannel.ConnectedUsers.Count > 0) continue;

                await ArchiveThread(thread, voiceChannel);
                continue;
            }

            await UpdateVoiceChannelUsers(thread, voiceChannel);
        }

        // 활성화 스레드 순회: 음성 채널 생성
        foreach (RestThreadChannel restThread in await temporaryChannel.GetActiveThreadsAsync())
        {
            if (IsThreadArchived(restThread))
            {
                // 스레드 비활성화
                await ArchiveThread(restThread);
                continue;
            }
            // 활성화 된 스레드의 음성채널 업데이트
            else
            {
                SocketVoiceChannel? voiceChannel = GetVoiceChannelByThreadName(restThread.Name);
                if (voiceChannel == null)
                {
                    // 음성채널 생성
                    await CreateVoiceChannelTo(restThread);
                }
            }
        }
    }

    #endregion


    #region 공개 함수

    public static async Task CreateTemporaryChannel(string name, IGuildUser creator)
    {
        SocketTextChannel managedThreadChannel = ManagerBotCore.Guild.GetTextChannel(Feature_PrivateChannel.Setting.ParentChannelId);
        SocketThreadChannel thread =
            await managedThreadChannel.CreateThreadAsync(
                name,
                autoArchiveDuration: DefaultThreadArchiveDuration,
                type: ThreadType.PrivateThread,
                invitable: true
            );
        await thread.AddUserAsync(creator);
    }
    public static async Task DeleteTemporaryChannel(SocketThreadChannel thread)
    {
        if (thread.ParentChannel.Id != Feature_PrivateChannel.Setting.ParentChannelId) return;

        // 음성 채널 삭제

        SocketVoiceChannel? voiceChannel = GetVoiceChannelByThreadName(thread.Name);
        if (voiceChannel != null)
        {
            await voiceChannel.DeleteAsync();
        }

        // 스레드 삭제
        await thread.DeleteAsync();
    }

    #endregion


    #region 도구

    #region 음성 채널 권한

    static async Task UpdateVoiceChannelUsers(SocketThreadChannel thread, SocketVoiceChannel voiceChannel)
    {
        // 유저 추가
        Stack<string> addedUserMentions = new Stack<string>();
        List<Overwrite> permissionOverwrites = new() {
            new Overwrite(
                ManagerBotCore.Guild.EveryoneRole.Id,
                PermissionTarget.Role,
                threadVoiceChannelEveryonePermissions
            )
        };
        foreach (SocketThreadUser user in thread.Users)
        {
            SocketGuildUser? guildUser = ManagerBotCore.Guild.GetUser(user.Id);
            if (guildUser == null) continue;
            if (guildUser.IsBot) continue;

            permissionOverwrites.Add(new Overwrite(
                user.Id,
                PermissionTarget.User,
                threadVoiceChannelUserPermissions
            ));

            if (voiceChannel.Users.Any(x => x.Id == user.Id)) continue;

            addedUserMentions.Push(user.Mention);
        }

        // 유저 제거
        Stack<string> removedUserMentions = new Stack<string>();
        foreach (IGuildUser user in voiceChannel.Users)
        {
            if (user.IsBot) continue;
            if (thread.Users.Any(x => x.Id == user.Id)) continue;

            removedUserMentions.Push(user.Mention);
        }

        await voiceChannel.ModifyAsync(
            x =>
            {
                x.PermissionOverwrites = permissionOverwrites;
                x.UserLimit = permissionOverwrites.Count - 1;
            }
        );

        await LogVoiceChannelUsersUpdate(thread, voiceChannel,
            addedUserMentions, removedUserMentions
        );
    }
    static async Task AddVoiceChannelUser(SocketVoiceChannel voiceChannel, IThreadUser user)
    {
        if (voiceChannel.Users.Any(x => x.Id == user.GuildUser.Id)) return;

        await voiceChannel.AddPermissionOverwriteAsync(user.GuildUser, threadVoiceChannelUserPermissions);

        await voiceChannel.ModifyAsync(
            x => x.UserLimit = voiceChannel.Users.Count()
        );

        await LogVoiceChannelAddUser(user.Thread, voiceChannel, user);
    }
    static async Task AddVoiceChannelUser(RestVoiceChannel restVoiceChannel, IThreadUser user)
    {
        if (await restVoiceChannel.GetUsersAsync().Flatten().AnyAsync(x => x.Id == user.GuildUser.Id)) return;

        await restVoiceChannel.AddPermissionOverwriteAsync(user.GuildUser, threadVoiceChannelUserPermissions);

        int userCount = await restVoiceChannel.GetUsersAsync().Flatten().CountAsync();
        await restVoiceChannel.ModifyAsync(
            x => x.UserLimit = userCount
        );

        await LogVoiceChannelAddUser(user.Thread, restVoiceChannel, user);
    }
    static async Task RemoveVoiceChannelUser(SocketVoiceChannel voiceChannel, IThreadUser user)
    {
        if (voiceChannel.Users.Any(x => x.Id == user.GuildUser.Id)) return;

        await voiceChannel.RemovePermissionOverwriteAsync(user.GuildUser);

        await voiceChannel.ModifyAsync(
            x => x.UserLimit = voiceChannel.Users.Count()
        );

        await LogVoiceChannelRemoveUser(user.Thread, voiceChannel, user);
    }
    static async Task RemoveVoiceChannelUser(RestVoiceChannel restVoiceChannel, IThreadUser user)
    {
        if (await restVoiceChannel.GetUsersAsync().Flatten().AnyAsync(x => x.Id == user.GuildUser.Id)) return;

        await restVoiceChannel.RemovePermissionOverwriteAsync(user.GuildUser);

        int userCount = await restVoiceChannel.GetUsersAsync().Flatten().CountAsync();
        await restVoiceChannel.ModifyAsync(
            x => x.UserLimit = userCount
        );

        await LogVoiceChannelRemoveUser(user.Thread, restVoiceChannel, user);
    }
    static List<Overwrite> GetThreadVoiceChannelPermissionOverwrites(SocketThreadChannel thread)
    {
        List<Overwrite> permissionOverwrites = new() {
            new Overwrite(
                ManagerBotCore.Guild.EveryoneRole.Id,
                PermissionTarget.Role,
                threadVoiceChannelEveryonePermissions
            )
        };
        foreach (SocketThreadUser user in thread.Users)
        {
            if (user == null) continue;
            if (user.IsBot) continue;

            permissionOverwrites.Add(new Overwrite(
                user.Id,
                PermissionTarget.User,
                threadVoiceChannelUserPermissions
            ));
        }
        return permissionOverwrites;
    }
    static async Task<List<Overwrite>> GetThreadVoiceChannelPermissionOverwrites(RestThreadChannel restThread)
    {
        List<Overwrite> permissionOverwrites = new() {
            new Overwrite(
                ManagerBotCore.Guild.EveryoneRole.Id,
                PermissionTarget.Role,
                threadVoiceChannelEveryonePermissions
            )
        };
        await foreach (RestThreadUser user in restThread.GetThreadUsersAsync().Flatten())
        {
            SocketGuildUser? guildUser = ManagerBotCore.Guild.GetUser(user.Id);
            if (guildUser == null) continue;
            if (guildUser.IsBot) continue;

            permissionOverwrites.Add(new Overwrite(
                user.Id,
                PermissionTarget.User,
                threadVoiceChannelUserPermissions
            ));
        }
        return permissionOverwrites;
    }

    #endregion


    #region 찾기

    static SocketVoiceChannel? GetVoiceChannelByThreadName(string name)
    {
        ulong voiceChannelId = ExtractID(name);
        if (voiceChannelId == 0) return null;

        return ManagerBotCore.Guild.GetVoiceChannel(voiceChannelId);
    }
    static SocketThreadChannel? GetThreadByVoiceChannelName(string name)
    {
        ulong threadId = ExtractID(name);
        if (threadId == 0) return null;

        return ManagerBotCore.Guild.GetThreadChannel(threadId);
    }

    #endregion


    #region 생성 및 삭제

    static async ValueTask<RestVoiceChannel> CreateVoiceChannelTo(SocketThreadChannel thread)
    {
        string threadName = ExtractName(thread.Name);
        List<Overwrite> permissionOverwrites = GetThreadVoiceChannelPermissionOverwrites(thread);

        RestVoiceChannel restVoiceChannel = await ManagerBotCore.Guild.CreateVoiceChannelAsync(
            threadName,
                x =>
                {
                    x.CategoryId = Feature_PrivateChannel.Setting.CategoryId;
                    x.PermissionOverwrites = permissionOverwrites;
                    x.Position = 1;
                }
            );
        await restVoiceChannel.SendMessageAsync(thread.Mention);
        await ChangeChannelIdInfo(thread, restVoiceChannel.Id);

        await LogCreateVoiceChannel(thread, restVoiceChannel);
        return restVoiceChannel;
    }
    static async ValueTask<RestVoiceChannel> CreateVoiceChannelTo(RestThreadChannel restThread)
    {
        string threadName = ExtractName(restThread.Name);
        List<Overwrite> permissionOverwrites = await GetThreadVoiceChannelPermissionOverwrites(restThread);

        RestVoiceChannel restVoiceChannel = await ManagerBotCore.Guild.CreateVoiceChannelAsync(
            threadName,
                x =>
                {
                    x.CategoryId = Feature_PrivateChannel.Setting.CategoryId;
                    x.PermissionOverwrites = permissionOverwrites;
                    x.Position = 1;
                }
            );
        await restVoiceChannel.SendMessageAsync(restThread.Mention);
        await ChangeChannelIdInfo(restThread, restVoiceChannel.Id);

        await LogCreateVoiceChannel(restThread, restVoiceChannel);
        return restVoiceChannel;
    }
    static async Task DeleteVoiceChannel(SocketThreadChannel? thread, SocketVoiceChannel? voiceChannel)
    {
        // 음성 채널 삭제
        if (voiceChannel != null)
        {
            await voiceChannel.DeleteAsync();

            await LogDeleteVoiceChannel(thread, voiceChannel);
        }
    }
    static async Task DeleteVoiceChannel(RestThreadChannel restThread, SocketVoiceChannel? voiceChannel)
    {
        // 음성 채널 삭제
        if (voiceChannel != null)
        {
            await voiceChannel.DeleteAsync();

            await LogDeleteVoiceChannel(restThread, voiceChannel);
        }
    }

    #endregion


    #region 스레드 보관

    static bool IsThreadArchived(SocketThreadChannel thread)
    {
        return thread.IsArchived
            || thread.IsLocked
            || thread.ArchiveTimestamp
                <= DateTimeOffset.Now.AddMinutes(-(double)thread.AutoArchiveDuration);
    }
    static bool IsThreadArchived(RestThreadChannel restThread)
    {
        return restThread.IsArchived
            || restThread.IsLocked
            || restThread.ArchiveTimestamp
                <= DateTimeOffset.Now.AddMinutes(-(double)restThread.AutoArchiveDuration);
    }

    /// <summary> 음성채널 탐색 및 삭제 </summary>
    static async Task ArchiveThread(SocketThreadChannel thread)
    {
        activeTemporaryChannels.Bin
        SocketVoiceChannel? voiceChannel = GetVoiceChannelByThreadName(thread.Name);
        if (voiceChannel == null) return;

        await ArchiveThread(thread, voiceChannel);
    }
    /// <summary> 음성채널 탐색 및 삭제 </summary>
    static async Task ArchiveThread(RestThreadChannel restThread)
    {
        SocketVoiceChannel? voiceChannel = GetVoiceChannelByThreadName(restThread.Name);
        if (voiceChannel == null) return;

        await ArchiveThread(restThread, voiceChannel);
    }

    /// <summary> 음성채널 삭제 </summary>
    static async Task ArchiveThread(SocketThreadChannel thread, SocketVoiceChannel voiceChannel)
    {
        if (voiceChannel.ConnectedUsers.Count > 0) return;

        if (thread.IsArchived == false)
        {
            await thread.ModifyAsync(
                x => x.Archived = true
            );
        }

        await DeleteVoiceChannel(thread, voiceChannel);
    }
    /// <summary> 음성채널 삭제 </summary>
    static async Task ArchiveThread(RestThreadChannel restThread, SocketVoiceChannel voiceChannel)
    {
        if (voiceChannel.ConnectedUsers.Count > 0) return;

        if (restThread.IsArchived == false)
        {
            await restThread.ModifyAsync(
                x => x.Archived = true
            );
        }

        await DeleteVoiceChannel(restThread, voiceChannel);
    }

    #endregion


    #region 채널 아이디 정보 변경

    static async Task ChangeChannelIdInfo(SocketThreadChannel thread, ulong id)
    {
        await voiceChannel.SendMessageAsync(MentionUtility.MentionChannel(id));
    }
    static async Task ChangeChannelIdInfo(RestThreadChannel thread, ulong id)
    {
        await voiceChannel.SendMessageAsync(MentionUtility.MentionChannel(id));
    }
    static async Task ChangeChannelIdInfo(SocketVoiceChannel voiceChannel, ulong id)
    {

        await voiceChannel.SendMessageAsync(MentionUtility.MentionChannel(id));
    }

    #endregion


    #region 디버그 텍스트

    static string GetThreadAndVoiceChannelNameAndMention(IChannel? thread, IChannel voiceChannel)
    {
        if (thread == null)
        {
            return GetChannelNameAndMention(voiceChannel);
        }
        else
        {
            return $"{GetChannelNameAndMention(voiceChannel)} : {GetChannelNameAndMention(thread)}";
        }
    }
    static string GetChannelNameAndMention(IChannel channel)
        => GetChannelNameAndMention(channel.Name, channel.Id);
    static string GetChannelNameAndMention(string name, ulong id)
    {
        return $"{MentionUtility.MentionChannel(id)} ({name})";
    }

    #endregion

    #endregion


    #region 로그

    #region 유저 업데이트 로그

    static async Task<RestUserMessage> LogVoiceChannelAddUser(IThreadChannel thread, IVoiceChannel voiceChannel, IThreadUser user)
    {
        return await GuildData.LogNormal(
            $"임시 음성 채널 사용자 업데이트",
            GetThreadAndVoiceChannelNameAndMention(thread, voiceChannel),
            new List<EmbedFieldBuilder>(){
                new EmbedFieldBuilder()
                {
                    Name = "추가된 사용자",
                    Value = user.Mention
                }
            }
        );
    }
    static async Task<RestUserMessage> LogVoiceChannelRemoveUser(IThreadChannel thread, IVoiceChannel voiceChannel, IThreadUser user)
    {
        return await GuildData.LogNormal(
            $"임시 음성 채널 사용자 업데이트",
            GetThreadAndVoiceChannelNameAndMention(thread, voiceChannel),
            new List<EmbedFieldBuilder>(){
                new EmbedFieldBuilder()
                {
                    Name = "추가된 사용자",
                    Value = user.Mention
                }
            }
        );
    }

    static async Task<RestUserMessage?> LogVoiceChannelUsersUpdate(IThreadChannel thread, IVoiceChannel voiceChannel,
        IEnumerable<string> addedUserMentions, IEnumerable<string> removedUserMentions)
    {
        List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
        if (addedUserMentions.Count() > 0)
        {
            fields.Add(new EmbedFieldBuilder()
            {
                Name = "추가된 사용자",
                Value = string.Join(", ", addedUserMentions)
            });
        }
        if (removedUserMentions.Count() > 0)
        {
            fields.Add(new EmbedFieldBuilder()
            {
                Name = "제거된 사용자",
                Value = string.Join(", ", removedUserMentions)
            });
        }
        if (fields.Count == 0) return null;

        return await GuildData.LogNormal(
            $"임시 음성 채널 사용자 업데이트",
            GetThreadAndVoiceChannelNameAndMention(thread, voiceChannel),
            fields
        );
    }

    #endregion


    #region 음성 채널 생성 및 삭제 로그

    static Task<RestUserMessage> LogCreateVoiceChannel(IThreadChannel thread, IVoiceChannel voiceChannel)
    {
        return GuildData.LogNormal(
            $"임시 음성 채널 생성",
            GetThreadAndVoiceChannelNameAndMention(thread, voiceChannel)
        );
    }
    static Task<RestUserMessage> LogDeleteVoiceChannel(IThreadChannel? thread, IVoiceChannel voiceChannel)
    {
        return GuildData.LogNormal(
            "스레드 음성 채널 삭제",
            GetThreadAndVoiceChannelNameAndMention(thread, voiceChannel)
        );
    }

    #endregion

    #endregion
}