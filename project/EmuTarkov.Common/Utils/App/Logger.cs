using System;

namespace EmuTarkov.Common.Utils.App
{
    public class Logger
    {
        private static void Log(string type, string text)
        {
            Console.WriteLine("{0} | {1}", type, text);
        }

        public static void LogData(string text)
        {
            Console.WriteLine("{0}", text);
        }

        public static void LogInfo(string text)
        {
            Log("INFO", text);
        }

        public static void LogWarning(string text)
        {
            Log("WARNING", text);
        }

        public static void LogError(string text)
        {
            Log("ERROR", text);
        }
    }
}
