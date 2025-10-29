using Discord;
using Discord.WebSocket;
using ManagerBot.Utils.Extension.Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerBot.SlashCommandSystem.DebugCommandGroup.Encoding;
public class Base64 : SubCommand
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

    public Base64(string name, string description) : base(name, description) { }

    protected override ValueTask OnSubCommandExecuted(SocketSlashCommand command, SocketSlashCommandDataOption parentData)
    {
        ulong id;
        SocketSlashCommandDataOption option = parentData.Options.First();
        switch (option.Type)
        {
            case ApplicationCommandOptionType.String:
                id = ulong.Parse((string)option.Value);
                break;
            case ApplicationCommandOptionType.User:
                id = ((IUser)option.Value).Id;
                break;
            case ApplicationCommandOptionType.Role:
                id = ((IRole)option.Value).Id;
                break;
            case ApplicationCommandOptionType.Channel:
                id = ((ISocketMessageChannel)option.Value).Id;
                break;
            default:
                _ = command.ErrorRespond($"형식이 잘못되었습니다. : {option.Value} ({option.Type})");
                return ValueTask.CompletedTask;
        }
        _ = command.RespondAsync(Convert.ToBase64String(BitConverter.GetBytes(id)));
        return ValueTask.CompletedTask;
    }
}
