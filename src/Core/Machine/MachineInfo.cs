using Discord;

namespace ManagerBot.Core.Machine;

public abstract class MachineInfo
{
    public static MachineInfo CreateMachineInfo()
    {
        PlatformID platform = Environment.OSVersion.Platform;

        switch (platform)
        {
            case PlatformID.Win32NT:
            {
                return new MachineInfo_Laptop();
            }
            case PlatformID.Unix:
            {
                return new MachineInfo_Raspberrypi();
            }
        }

        throw new NotSupportedException("지원되지 않는 머신입니다.");
    }


    public virtual string DirectoryName { get; } = Directory.GetCurrentDirectory();
    public virtual DriveInfo driveInfo { get; } = new DriveInfo(Directory.GetCurrentDirectory());


    public virtual PlatformID PlatformID => Environment.OSVersion.Platform;
    public virtual int ProcessorCount => Environment.ProcessorCount;

    public virtual long AvailableDiskSpace => driveInfo.AvailableFreeSpace;
    public virtual long TotalDiskSpace => driveInfo.TotalSize;


    public virtual string OperatingSystemName { get; protected set; } = Environment.OSVersion.ToString();

    public virtual long UsingMemory { get; protected set; } = -1;


    public abstract MachineType MachineType { get; }
    public abstract ulong AvailableMemory { get; }
    public abstract ulong TotalMemory { get; }


    public DateTime LastUpdateUtcTime { get; private set; } = DateTime.MinValue;


    public MachineInfo()
    {

    }

    public void UpdateMachineInfo()
    {
        Update();


        LastUpdateUtcTime = DateTime.UtcNow;
    }
    protected virtual void Update()
    {
        UsingMemory = GC.GetTotalMemory(false);
    }

    public abstract EmbedFieldBuilder GetMachineInfoEmbedFieldBuilder();
}