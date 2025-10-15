using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;

namespace ManagerBot.Core;

public class Program
{
    public static async Task Main(string[] args)
    {
        PathHelper.SetDataDirectoryPath(args[0]);

        await ManagerBotCore.Initialize();

        while (true)
        {
            string? command = Console.ReadLine();
            if (command == null)
                continue;

            if (command == "stop")
            {
                Console.WriteLine("Stopping the bot...");
                break;
            }
        }
    }
}
