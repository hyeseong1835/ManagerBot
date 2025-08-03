namespace ManagerBot.Core;

public class Program
{
    public static async Task Main(string[] args)
    {
        PathHelper.dataDirectoryPath = args[0];

        await ManagerBotCore.Initialize();

        await ManagerBotCore.Stop();
    }
}