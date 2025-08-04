using Discord;
using ManagerBot.Core.Machine;

public class MachineInfo_Raspberrypi : MachineInfo
{
    public override MachineType MachineType => MachineType.Raspberrypi;

    public override ulong AvailableMemory => _memStatus.ullAvailPhys;
    public override ulong TotalMemory => _memStatus.ullTotalPhys;

    readonly object _winMemoryLock = new();
    readonly MEMORYSTATUSEX _memStatus = new();


    public MachineInfo_Raspberrypi()
        : base()
    {
        lock (_winMemoryLock)
        {
            MEMORYSTATUSEX.GlobalMemoryStatusEx(_memStatus);
        }
    }

    protected override void Update()
    {

    }

    public override EmbedFieldBuilder GetMachineInfoEmbedFieldBuilder()
    {
        return new EmbedFieldBuilder()
        {
            Name = "Raspberrypi 머신 정보",
            Value = $"운영체제: {Environment.OSVersion}" +
                    $"\n메모리: {MemorySizeUtility.BToGB(AvailableMemory)}/{MemorySizeUtility.BToGB(TotalMemory)} GB" +
                    $"\n디스크: {MemorySizeUtility.BToGB(AvailableDiskSpace)}/{MemorySizeUtility.BToGB(TotalDiskSpace)} GB"
        };
    }
}