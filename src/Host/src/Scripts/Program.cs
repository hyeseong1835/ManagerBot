using System.Threading.Tasks;

namespace ManagerBot.Core;

public class Program
{
    public static async Task Main(string[] args)
    {
        // 데이터 디렉토리 설정
        PathHelper.SetDataDirectoryPath(args[0]);

        // 매니저봇
        await ManagerBotCore.Initialize();

        // 커맨드
        await ConsoleCommand.StartRead();
    }
}
