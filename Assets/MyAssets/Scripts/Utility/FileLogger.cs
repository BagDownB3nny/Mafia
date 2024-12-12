using System.IO;
using Mirror;
using UnityEngine;

public static class FileLogger
{
    public static readonly string LogFilePath = Path.Combine(Application.persistentDataPath, "debug.txt");

    public static void Log(string message)
    {
        // Write the log message to the file with a timestamp
        File.AppendAllText(LogFilePath, $"{System.DateTime.Now}: {message}\n");
    }

    public static void LogWarning(string message)
    {
        File.AppendAllText(LogFilePath, $"{System.DateTime.Now} [WARNING]: {message}\n");
    }

    public static void LogError(string message)
    {
        File.AppendAllText(LogFilePath, $"{System.DateTime.Now} [ERROR]: {message}\n");
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
