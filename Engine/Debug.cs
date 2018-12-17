/*
	Debug.cs
        By: Jakub P. Szarkowicz / JakubSzark
*/

using System;

namespace PGE
{
    public class Debug
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
    }

    public enum LogLevel
    {
        INFO,
        WARNING,
        ERROR
    }
}