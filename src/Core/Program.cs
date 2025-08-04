namespace ManagerBot.Core;


Console.WriteLine($"총 메모리: {totalMemory / (1024 * 1024)} MB");

public class Program
{
    public static readonly MachineType machineType = Environment.OSVersion.Platform switch
    {
        PlatformID.Win32NT => MachineType.Windows,
        PlatformID.Unix => MachineType.Raspberrypi,
        _ => throw new NotSupportedException("Unsupported platform.")
    };

    public static async Task Main(string[] args)
    {
        PathHelper.SetDataDirectoryPath(args[0]);



        await ManagerBotCore.Initialize();

        await Task.Delay(-1);

    }
}