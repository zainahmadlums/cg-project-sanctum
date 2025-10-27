using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

/// <summary>
/// Forces a script refresh ONLY when scripts have changed since last domain reload.
/// Works even with Auto Refresh disabled.
/// </summary>
[InitializeOnLoad]
public static class SmartRefreshOnPlay
{
    private static string cacheFilePath = "Library/LastScriptRefreshTimestamp.txt";

    static SmartRefreshOnPlay()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            if (HaveScriptsChanged())
            {
                Debug.Log("🌀 Scripts changed since last play → refreshing assets before Play...");
                AssetDatabase.Refresh();
                WriteCurrentScriptTimestamp();
            }
            else
            {
                Debug.Log("✅ No script changes detected → skipping refresh.");
            }
        }
    }

    private static bool HaveScriptsChanged()
    {
        // Get all .cs files in Assets folder
        var allScripts = Directory.GetFiles("Assets", "*.cs", SearchOption.AllDirectories);
        if (allScripts.Length == 0) return false;

        // Read last recorded timestamp
        long lastTimestamp = ReadLastScriptTimestamp();

        // Check if any file is newer than the last refresh timestamp
        return allScripts.Any(file => File.GetLastWriteTimeUtc(file).Ticks > lastTimestamp);
    }

    private static long ReadLastScriptTimestamp()
    {
        if (!File.Exists(cacheFilePath))
            return 0;

        string content = File.ReadAllText(cacheFilePath);
        if (long.TryParse(content, out long result))
            return result;

        return 0;
    }

    private static void WriteCurrentScriptTimestamp()
    {
        long newestTime = Directory.GetFiles("Assets", "*.cs", SearchOption.AllDirectories)
            .Select(f => File.GetLastWriteTimeUtc(f).Ticks)
            .DefaultIfEmpty(0)
            .Max();

        File.WriteAllText(cacheFilePath, newestTime.ToString());
    }
}
