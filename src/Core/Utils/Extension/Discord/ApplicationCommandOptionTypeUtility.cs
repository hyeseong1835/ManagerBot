using Discord.WebSocket;

namespace Discord;
public static class ApplicationCommandOptionTypeUtility
{
    public readonly static Dictionary<Type, ApplicationCommandOptionType> typeDictionary = new Dictionary<Type, ApplicationCommandOptionType>
    {
        //1. SubCommand

        //2. SubCommandGroup

        //3. String
        { typeof(string), ApplicationCommandOptionType.String },

        //4. Integer
        { typeof(long), ApplicationCommandOptionType.Integer },
        //{ typeof(int), ApplicationCommandOptionType.Integer },

        //5. Boolean
        { typeof(bool), ApplicationCommandOptionType.Boolean },

        //6. User
        { typeof(IUser), ApplicationCommandOptionType.User },

        { typeof(SocketUser), ApplicationCommandOptionType.User },

        //7. Channel
        { typeof(IGuildChannel), ApplicationCommandOptionType.Channel },

        { typeof(ISocketMessageChannel), ApplicationCommandOptionType.Channel },

        //8. Role
        { typeof(IRole), ApplicationCommandOptionType.Role },

        //9. Mentionable
        //{ typeof(IUser), ApplicationCommandOptionType.Mentionable },
        //{ typeof(IRole), ApplicationCommandOptionType.Mentionable },

        //10. Number
        { typeof(double), ApplicationCommandOptionType.Number },

        //{ typeof(float), ApplicationCommandOptionType.Number },

        //11. Attachment
        { typeof(IAttachment), ApplicationCommandOptionType.Attachment }
    };
}