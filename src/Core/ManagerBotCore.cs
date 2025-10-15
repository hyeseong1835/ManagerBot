using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ManagerBot.Utils.PriorityMethod;

namespace ManagerBot.Core;

public class OnBotInitializeMethodAttribute : PriorityMethodAttribute
{
    public OnBotInitializeMethodAttribute(int priority = 100) : base(priority) { }
}

public class OnBotStopMethodAttribute : PriorityMethodAttribute
{
    public OnBotStopMethodAttribute(int priority = 100) : base(priority) { }
}

public static class ManagerBotCore
{
    public readonly static DiscordSocketClient client = new DiscordSocketClient(
        new DiscordSocketConfig
        {
            MessageCacheSize = 100,
            GatewayIntents = GatewayIntents.All,
            AuditLogCacheSize = 0,
            LogLevel = LogSeverity.Info
        }
    );
    public readonly static CommandService commandService = new CommandService(
        new CommandServiceConfig()
        {
            LogLevel = LogSeverity.Info
        }
    );

    static ManagerBotSetting? setting;
    public static ManagerBotSetting Setting => setting!;

    static SocketGuild? guild;
    public static SocketGuild Guild => guild!;

    public static async ValueTask Initialize()
    {
        // 설정 로드
        setting = await ManagerBotSetting.LoadAsync();

        // 봇 로그인
        await client.LoginAsync(TokenType.Bot, setting.BotToken);
        await client.StartAsync();

        // Ready 이벤트 발생 대기용 TaskCompletionSource
        var readyTcs = new TaskCompletionSource();

        client.Ready += async () =>
        {
            try
            {
                // 길드 로드
                guild = client.GetGuild(setting.GuildId)
                ?? throw new InvalidOperationException($"길드 ID '{setting.GuildId}'에 해당하는 길드를 찾을 수 없습니다.");

                Console.WriteLine($"길드 로드: {guild.Name} ({guild.Id})");

                // 로드
                await PriorityMethodAttribute.FindAndInvoke<OnBotInitializeMethodAttribute>();

                // Ready 이벤트 완료 신호
                readyTcs.TrySetResult();
            }
            catch (Exception ex)
            {
                // 예외 발생 시 TCS 실패 처리
                readyTcs.TrySetException(ex);
            }
        };

        client.Log += (log) =>
        {
            Console.WriteLine($"[{log.Severity}] {log.Source}: {log.Message}");
            if (log.Exception != null)
                Console.WriteLine(log.Exception);
            return Task.CompletedTask;
        };

        // Ready 이벤트까지 대기
        await readyTcs.Task;
    }
    public static async ValueTask Stop()
    {
        // 봇 종료 이벤트 발생
        await PriorityMethodAttribute.FindAndInvoke<OnBotStopMethodAttribute>();

        // 설정 저장
        if (setting != null)
            await ManagerBotSetting.SaveAsync(setting);

        // 봇 로그아웃
        await client.LogoutAsync();
    }
}
