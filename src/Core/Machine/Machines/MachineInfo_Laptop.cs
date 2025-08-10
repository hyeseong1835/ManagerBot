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
                    $"\nCPU 사용 시간: {TotalCpuTime.Hours}시간 {TotalCpuTime.Minutes}분 {TotalCpuTime.Seconds}초" +
                      $"\n메모리: {(MemorySizeUtility.BToGiB(UsedMemory)).ToString("N2")} GiB / {MemorySizeUtility.BToGiB(TotalMemory).ToString("N2")} GiB" +
                      $"\n디스크: {MemorySizeUtility.BToGiB(UsedDiskSpace).ToString("N2")} GiB / {MemorySizeUtility.BToGiB(TotalDiskSpace).ToString("N2")} GiB"
        };
    }
}