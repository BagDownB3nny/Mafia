using System;
using System.IO;
using Mirror;
using UnityEngine;

public static class FileLogger
{
    public static readonly string LogFilePath;

    static FileLogger()
    {
        try
        {
            // 1. Get game directory
            string gameDir = Directory.GetParent(Application.dataPath)?.FullName;
            
            // 2. Ensure directory exists
            if (!Directory.Exists(gameDir))
                Directory.CreateDirectory(gameDir);
            
            // 3. Set path
            LogFilePath = Path.Combine(gameDir, "debug.txt");
            
            // 4. Initialize file with header
            File.WriteAllText(LogFilePath, $"=== DEBUG LOG STARTED {DateTime.Now} ===\n\n");
            
            Debug.Log($"Log file initialized at: {LogFilePath}");
        }
        catch (Exception ex)
        {
            // Fallback to persistentDataPath
            LogFilePath = Path.Combine(Application.persistentDataPath, "debug.txt");
            Debug.LogError($"Failed to create log in game dir: {ex.Message}. Using fallback: {LogFilePath}");
        }
    }

    public static void Log(string message)
    {
        try {
            File.AppendAllText(LogFilePath, $"{System.DateTime.Now}: {message}\n");
        }
        catch (Exception ex)
        {
            // Dont debug log error or it might cause a loop
            // Debug.LogError($"Failed to write log: {ex.Message}");
        }
    }

    public static void LogWarning(string message)
    {
        try {
            File.AppendAllText(LogFilePath, $"{System.DateTime.Now} [WARNING]: {message}\n");
        }
        catch (Exception ex)
        {
            // Dont debug log error or it might cause a loop
            // Debug.LogError($"Failed to write log: {ex.Message}");
        }
    }

    public static void LogError(string message)
    {
        try {
            File.AppendAllText(LogFilePath, $"{System.DateTime.Now} [ERROR]: {message}\n");
        }
        catch (Exception ex)
        {
            // Dont debug log error or it might cause a loop
            // Debug.LogError($"Failed to write log: {ex.Message}");
        }
    }

    public static string DebugDictLog<TKey, TValue>(string dictName, SyncDictionary<TKey, TValue> dict)
    {
        string log = "dictName: \n";
        foreach (var key in dict.Keys)
        {
            log += $"{key}: {dict[key]}\n";
        }
        return log;
    }
}
