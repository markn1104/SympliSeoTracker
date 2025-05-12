using System;
using SympliSeoTracker.Domain.Interfaces;

namespace SympliSeoTracker.Infrastructure.Logging
{
    public class LoggerManager : ILoggerManager
    {        
        public void LogDebug(string message)
        {
            Console.WriteLine($"DEBUG: {DateTime.Now} - {message}");
        }

        public void LogError(string message)
        {
            Console.WriteLine($"ERROR: {DateTime.Now} - {message}");
        }

        public void LogInfo(string message)
        {
            Console.WriteLine($"INFO: {DateTime.Now} - {message}");
        }

        public void LogWarning(string message)
        {
            Console.WriteLine($"WARNING: {DateTime.Now} - {message}");
        }
    }
}