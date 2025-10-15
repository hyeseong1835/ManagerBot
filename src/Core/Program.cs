using ManagerBot.Core.Machine;

namespace ManagerBot.Core;


public class Program
{
    public static readonly MachineInfo machineInfo = MachineInfo.CreateMachineInfo();

    public static async Task Main(string[] args)
    {
        PathHelper.SetDataDirectoryPath(args[0]);

        await ManagerBotCore.Initialize();

        while (true)
        {
            string? command = Console.ReadLine();
            if (command == null)
                continue;

            if (command == "stop")
            {
                Console.WriteLine("Stopping the bot...");
                break;
            }
        }
    }
}
