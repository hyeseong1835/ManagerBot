using Discord;
using Discord.WebSocket;
using ManagerBot.Utils.Extension.Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerBot.SlashCommandSystem.DebugCommandGroup.Encoding;
public class KoreanString : SubCommand
{
    public override List<SlashCommandOptionBuilder>? Options => new List<SlashCommandOptionBuilder>
    {
        new SlashCommandOptionBuilder()
        {
            Name = "idstring",
            Description = "Base64로 변환할 id",
            Type = ApplicationCommandOptionType.String,
        },
        new SlashCommandOptionBuilder()
        {
            Name = "userid",
            Description = "유저 id",
            Type = ApplicationCommandOptionType.User,
        },
        new SlashCommandOptionBuilder()
        {
            Name = "roleid",
            Description = "역할 id",
            Type = ApplicationCommandOptionType.Role,
        },
        new SlashCommandOptionBuilder()
        {
            Name = "channelid",
            Description = "채널 id",
            Type = ApplicationCommandOptionType.Channel,
        }
    };

    public KoreanString(string name, string description) : base(name, description) { }

    protected override ValueTask OnSubCommandExecuted(SocketSlashCommand command, SocketSlashCommandDataOption parentData)
    {
        ulong id;
        SocketSlashCommandDataOption option = parentData.Options.First();

        switch (option.Type)
        {
            case ApplicationCommandOptionType.String:
                id = ulong.Parse((string)option.Value);
                break;
            default:
                if (!(option.Value is IEntity<ulong> entity)) {
                    _ = command.ErrorRespond($"형식이 잘못되었습니다. : {option.Value} ({option.Type})");
                    return ValueTask.CompletedTask;
                }

                id = entity.Id;
                break;
        }
        _ = command.RespondAsync(ManagerBot.Utils.Encodings.KoreanString.UlongToKoreanString(id));
        return ValueTask.CompletedTask;
    }
}
