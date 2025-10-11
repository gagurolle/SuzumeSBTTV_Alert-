using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamerBotSuzume
{
    public static class Logger
    {
        public static string? logFilePath { get; set; } = "log.txt";
        public static bool enableLogging { get; set; } = true;


        public static void LogError(string message)
        {

            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"ERROR {DateTime.Now}: {message}");
            }
        }
        public static void LogMessage(string message)
        {
            try
            {
                if (enableLogging)
                {
                    using (StreamWriter writer = new StreamWriter(logFilePath, true))
                    {
                        writer.WriteLine($"{DateTime.Now}: {message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to log message: {ex.Message}");
            }
        }
    }
}
