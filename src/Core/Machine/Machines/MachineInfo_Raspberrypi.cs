using Discord;
using ManagerBot.Core.Machine;

public class MachineInfo_Raspberrypi : MachineInfo
{
    public override MachineType MachineType => MachineType.Raspberrypi;

    public override ulong AvailableMemory => _memStatus.ullAvailPhys;
    public override ulong TotalMemory => _memStatus.ullTotalPhys;

    public MachineInfo_Raspberrypi()
        : base()
    {
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

    static string GetTempString(byte[] buffer)
    {
        FileStream tempFileStream = new FileStream("/sys/class/thermal/thermal_zone0/temp", FileMode.Open, FileAccess.Read);
        int readBytes = tempFileStream.Read(buffer);

        Span<char> charSpan = stackalloc char[readBytes + 2];

        buffer.AsSpan(0, readBytes - 1).WriteToCharSpan(charSpan);
        tempFileStream.Dispose();

        charSpan.Insert('.', readBytes - 3);
        charSpan[^2] = '°';
        charSpan[^1] = 'C';

        return new string(charSpan);
    }

    static string GetMemoryUsageString(byte[] buffer)
    {
        long memAvailable = -1;
        long memTotal = -1;

        using FileStream stream = new FileStream("/proc/meminfo", FileMode.Open, FileAccess.Read);
        {
            KeyValueTextReader reader = new KeyValueTextReader(stream, buffer);
            while (reader.ReadKey())
            {
                ReadOnlySpan<byte> key = reader.CurTokenSpan;

                if (key.SequenceEqual("MemAvailable"u8))
                {
                    reader.ReadValue();
                    memAvailable = long.Parse(reader.CurTokenSpan.Slice(0, reader.CurTokenLength - 3)) / 1024 / 1024;
                    continue;
                }
                if (key.SequenceEqual("MemTotal"u8))
                {
                    reader.ReadValue();
                    memTotal = long.Parse(reader.CurTokenSpan.Slice(0, reader.CurTokenLength - 3)) / 1024 / 1024;
                    continue;
                }
            }
        }

        long memUsed = memTotal - memAvailable;



        int byteWritten;
        Span<char> memUsedChars = stackalloc char[15];
        memUsed.TryFormat(memUsedChars, out byteWritten);
        memUsedChars.Insert('.', byteWritten - 3);
        memUsedChars = memUsedChars.Slice(0, byteWritten + 1);
        Console.WriteLine(memUsedChars.Length);

        Span<char> memTotalChars = stackalloc char[15];
        memTotal.TryFormat(memTotalChars, out byteWritten);
        memTotalChars.Insert('.', byteWritten - 3);
        memTotalChars = memTotalChars.Slice(0, byteWritten + 1);
        Console.WriteLine(memTotalChars.Length);


        int charIndex = 0;
        Span<char> charSpan = stackalloc char[memUsedChars.Length + memTotalChars.Length + 7];


        // 사용 가능 메모리
        Console.WriteLine(new string(memTotalChars));
        Console.WriteLine(memTotalChars.Length);
        Console.WriteLine(charIndex);
        Console.WriteLine(charSpan.Length);
        //Span<char> c1 = charSpan.Slice(charIndex, memUsedChars.Length);
        //memUsedChars.CopyTo(c1);
        charIndex += memUsedChars.Length;

        charSpan[charIndex++] = 'G';
        charSpan[charIndex++] = 'i';
        charSpan[charIndex++] = 'B';

        charSpan[charIndex++] = '/';

        // 총 메모리
        Console.WriteLine(new string(memTotalChars));
        Console.WriteLine(memTotalChars.Length);
        Console.WriteLine(charIndex);
        Console.WriteLine(charSpan.Length);
        //Span<char> c2 = charSpan.Slice(charIndex, memTotalChars.Length);
        //memTotalChars.CopyTo(c2);
        charIndex += memTotalChars.Length;

        charSpan[charIndex++] = 'G';
        charSpan[charIndex++] = 'i';
        charSpan[charIndex++] = 'B';

        return new string(charSpan);
    }
}