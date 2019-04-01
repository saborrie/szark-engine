using System;
using System.Diagnostics;

namespace Szark
{
    public static class Debug
    {
        /// <summary>
        /// Logs a message to the console
        /// </summary>
        /// <param name="text">The Message</param>
        /// <param name="level">Level of Importance</param>
        public static void Log(string text, LogLevel level = LogLevel.INFO)
        {
            switch(level)
            {
                case LogLevel.INFO:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"[INFO]: {text}");
                    break;

                case LogLevel.WARNING:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[WARNING]: {text}");
                    break;

                case LogLevel.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[ERROR]: {text}");
                    break;
            }

            Console.ResetColor();
        }

        /// <summary>
        /// Creates a Stopwatch and times the action provided
        /// </summary>
        /// <param name="action">The action to be performed</param>
        /// <returns>The time it took in milliseconds</returns>
        public static float Stopwatch(Action action)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
    }

    public enum LogLevel
    {
        INFO,
        WARNING,
        ERROR
    }
}