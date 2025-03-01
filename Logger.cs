using System;
using System.IO;
using System.Text;
using System.Threading;

public class Logger : ILogger
{
    // Log directory
    private readonly string logDirectory;
    // Lock object for thread safety
    private readonly object lockObj = new object();

    // Singleton instance using the interface
    private static readonly Lazy<ILogger> instance = new Lazy<ILogger>(() => new Logger());

    // Private constructor to prevent direct instantiation
    private Logger()
    {
        logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");

        // Create the log directory if it doesn't exist
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }
    }

    // Public method to get the singleton instance
    public static ILogger Instance => instance.Value;

    // Log a message to the current date's log file
    public void Log(string level, string message)
    {
        // Construct the log message with timestamp and level
        string logMessage = $"[{DateTime.Now}] [{level}] {message}\n";

        // Get the current date's log file path
        string logFilePath = Path.Combine(logDirectory, $"{DateTime.Now:yyyy-MM-dd}.log");

        // Write the log message to the file
        lock (lockObj) // Ensure thread safety
        {
            File.AppendAllText(logFilePath, logMessage, Encoding.UTF8);
        }

        // Also write to the console for real-time monitoring
        Console.WriteLine(logMessage);
    }

    // Convenience methods for different log levels
    public void Info(string message)
    {
        Log("INFO", message);
    }

    public void Warning(string message)
    {
        Log("WARNING", message);
    }

    public void Error(string message)
    {
        Log("ERROR", message);
    }
}
