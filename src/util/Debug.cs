using System;
using System.Diagnostics;

namespace Szark
{
    public static class Debug
    {
        /// <summary>
        /// Logs a message to the console
        /// </summary>
        public static void Log(object obj, LogLevel level = LogLevel.INFO)
        {
            switch(level)
            {
                case LogLevel.INFO:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"[INFO]: {obj.ToString()}");
                    break;

                case LogLevel.WARNING:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[WARNING]: {obj.ToString()}");
                    break;

                case LogLevel.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[ERROR]: {obj.ToString()}");
                    break;
            }

            Console.ResetColor();
        }

        /// <summary>
        /// Creates a Stopwatch and times the action
        /// </summary>
        public static TimeSpan Stopwatch(Action action)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
    }

    public enum LogLevel
    {
        INFO,
        WARNING,
        ERROR
    }
}