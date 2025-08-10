using Discord;
using HS.Common.System.KeyValueText;
using ManagerBot.Core.Machine;

public class MachineInfo_Raspberrypi : MachineInfo
{
    public override MachineType MachineType => MachineType.Raspberrypi;

    ulong temp;
    public ulong Temperature => temp;

    ulong memAvailable;
    public override ulong AvailableMemory => memAvailable;

    ulong memTotal;
    public override ulong TotalMemory => memTotal;

    public MachineInfo_Raspberrypi()
        : base()
    {

    }

    protected override void Update()
    {
        byte[] buffer = new byte[1024];

        ReadTemp(buffer);
        ReadMemInfo(buffer);
    }

    public override EmbedFieldBuilder GetMachineInfoEmbedFieldBuilder()
    {
        return new EmbedFieldBuilder()
        {
            Name = "Raspberrypi 머신 정보",
            Value = $"운영체제: {Environment.OSVersion}" +
                    $"\nCPU 사용 시간: {TotalCpuTime.Hours}시간 {TotalCpuTime.Minutes}분 {TotalCpuTime.Seconds}초" +
                      $"\n메모리: {(MemorySizeUtility.BToGiB(UsedMemory)).ToString("N2")} GiB / {MemorySizeUtility.BToGiB(TotalMemory).ToString("N2")} GiB" +
                      $"\n디스크: {MemorySizeUtility.BToGiB(UsedDiskSpace).ToString("N2")} GiB / {MemorySizeUtility.BToGiB(TotalDiskSpace).ToString("N2")} GiB"
        };
    }

    void ReadTemp(byte[] buffer)
    {
        FileStream tempFileStream = new FileStream("/sys/class/thermal/thermal_zone0/temp", FileMode.Open, FileAccess.Read);
        int readBytes = tempFileStream.Read(buffer);

        temp = ulong.Parse(buffer.AsSpan(0, readBytes - 1));
    }

    void ReadMemInfo(byte[] buffer)
    {
        using FileStream stream = new FileStream("/proc/meminfo", FileMode.Open, FileAccess.Read);
        {
            KeyValueTextReader reader = new KeyValueTextReader(stream, buffer);
            while (reader.ReadKey())
            {
                ReadOnlySpan<byte> key = reader.CurTokenSpan;

                if (key.SequenceEqual("MemAvailable"u8))
                {
                    reader.ReadValue();
                    memAvailable = ulong.Parse(reader.CurTokenSpan.Slice(0, reader.CurTokenLength - 3)) / 1024 / 1024;
                    continue;
                }
                if (key.SequenceEqual("MemTotal"u8))
                {
                    reader.ReadValue();
                    memTotal = ulong.Parse(reader.CurTokenSpan.Slice(0, reader.CurTokenLength - 3)) / 1024 / 1024;
                    continue;
                }
            }
        }
    }
}