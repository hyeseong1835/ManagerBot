using Discord;
using ManagerBot.Core.Machine;

public class MachineInfo_Laptop : MachineInfo
{
    public override MachineType MachineType => MachineType.Laptop;

    public override ulong AvailableMemory => _memStatus.ullAvailPhys;
    public override ulong TotalMemory => _memStatus.ullTotalPhys;

    readonly object _winMemoryLock = new();
    readonly MEMORYSTATUSEX _memStatus = new();


    public MachineInfo_Laptop()
        : base()
    {
        lock (_winMemoryLock)
        {
            MEMORYSTATUSEX.GlobalMemoryStatusEx(_memStatus);
        }
    }

    protected override void Update()
    {
        lock (_winMemoryLock)
        {
            MEMORYSTATUSEX.GlobalMemoryStatusEx(_memStatus);
        }
    }

    public override EmbedFieldBuilder GetMachineInfoEmbedFieldBuilder()
    {
        return new EmbedFieldBuilder()
        {
            Name = "Laptop 머신 정보",
            Value = $"운영체제: {OperatingSystemName}" +
                    $"\nCPU: {ProcessorCount} 코어" +
                    $"\nCPU 사용 시간: {Environment.CpuUsage.TotalTime}" +
                    $"\n메모리: {MemorySizeUtility.BToGB(AvailableMemory)}/{MemorySizeUtility.BToGB(TotalMemory)} GB" +
                    $"\n디스크: {MemorySizeUtility.BToGB(AvailableDiskSpace)}/{MemorySizeUtility.BToGB(TotalDiskSpace)} GB"
        };
    }
}