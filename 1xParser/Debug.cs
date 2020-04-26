using System;
using System.IO;

namespace _1xParser
{
    static class Debug
    {
        const string logsFileName = "logs.txt";
        public static void LogException(Exception e)
        {
            if (!(e.Message.Contains("Thread") || e.Message.Contains("Поток")))
            {
                LogError(e.StackTrace.Replace(" в ", Environment.NewLine + "\t в "));
                LogError(e.Message);
            }
        }
        public static void LogWarning(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteLine(msg);
            SaveToFileErrsAndWarnings(msg);
        }
        public static void LogError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine(msg);
            SaveToFileErrsAndWarnings(msg);
        }
        public static void Log(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLine(msg);
        }

        static void WriteLine(string msg)
        {
            Console.WriteLine(GetConsolePrefix() + msg);
        }
        static string GetConsolePrefix()
        {
            return DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " ";
        }
        static void SaveToFileErrsAndWarnings(string msg)
        {
            try
            {
                File.AppendAllText(logsFileName, GetConsolePrefix() + msg + Environment.NewLine);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                WriteLine(e.Message);
            }
        }
    }
}
