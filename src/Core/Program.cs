namespace ManagerBot.Core;

public class Program
{
    public static async Task Main(string[] args)
    {
        await ManagerBotCore.Initialize();
        await Feature.Initialize();
    }
}