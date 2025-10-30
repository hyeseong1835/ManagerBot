using System;

namespace ManagerBot.Core;

public static class Debug
{
    public static void Log(string caller, string message, string? detail = null)
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[{caller}] {message}");
    }
    public static void LogWarning(string caller, string message, string? detail = null)
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[{caller}] {message}");
        if (detail != null)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(detail);
        }
    }
    public static void LogWarning(string caller, Exception e)
        => LogWarning(caller, $"{e.GetType().Name}: {e.Message}", e.StackTrace);

    public static void LogError(string caller, string message, string? detail = null)
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[{caller}] {message}");
        if (detail != null)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(detail);
        }
    }
    public static void LogError(string caller, Exception e)
        => LogError(caller, $"{e.GetType().Name}: {e.Message}", e.StackTrace);
}
