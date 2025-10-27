using System;
using System.Threading.Tasks;

namespace ManagerBot.Core;

public static class Debug
{
    public SerialQueue
    public static void LogAsync(string caller, string message, string? detail = null)
    {
        Console.WriteLine($"[{caller}] {message}");
    }
    public static void LogErrorAsync(string caller, string message, string? detail = null)
    {
        Console.Error.WriteLine($"[{caller}] {message}");
    }
}
