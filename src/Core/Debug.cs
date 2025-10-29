using System;
using System.Threading.Tasks;

namespace ManagerBot.Core;

public static class Debug
{
    public static void Log(string caller, string message, string? detail = null)
    {
        Console.WriteLine($"[{caller}] {message}");
    }
    public static void LogError(string caller, string message, string? detail = null)
    {
        Console.Error.WriteLine($"[{caller}] {message}");
    }
    public static void LogError(string caller, Exception e)
    {
        Console.Error.WriteLine($"[{caller}] {e.GetType().Name}{e.Message}\n{e.StackTrace}");
    }
}
