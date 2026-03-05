using System;
using System.IO;

namespace Marketplace.Wpf.Infrastructure;

public static class Logger
{
    private static readonly object _sync = new object();
    private static string _path = Path.Combine(AppContext.BaseDirectory, "marketplace.log");

    public static void Init(string? path = null)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(path)) _path = path!;
            var dir = Path.GetDirectoryName(_path) ?? AppContext.BaseDirectory;
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            Log("Logger initialized");
        }
        catch
        {
            // Ничего не делаем — логирование необязательное
        }
    }

    public static void Log(string message)
    {
        try
        {
            lock (_sync)
            {
                File.AppendAllText(_path, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}{Environment.NewLine}");
            }
        }
        catch
        {
            // silent
        }
    }

    public static void LogException(Exception? ex, string? context = null)
    {
        try
        {
            if (ex == null)
            {
                Log((context ?? "") + " Null exception");
                return;
            }

            var header = context != null ? $"EXCEPTION ({context})" : "EXCEPTION";
            Log($"{header}: {ex.GetType()}: {ex.Message}");
            Log(ex.StackTrace ?? string.Empty);
            if (ex.InnerException != null)
            {
                LogException(ex.InnerException, "Inner");
            }
        }
        catch
        {
            // silent
        }
    }
}
