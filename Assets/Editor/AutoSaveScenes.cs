using UnityEditor;
using UnityEngine;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class AutoSaveScenes
{
    private static DateTime nextSaveTime;

    static AutoSaveScenes()
    {
        nextSaveTime = DateTime.Now.AddMinutes(10);
        EditorApplication.update += OnEditorUpdate;
        AutoSave();
        Debug.Log("AutoSave active: saves open scenes every 10 minutes.(Press ctrl+r to refresh scripts)");
    }

    private static void OnEditorUpdate()
    {
        if (DateTime.Now >= nextSaveTime)
        {
            AutoSave();
            nextSaveTime = DateTime.Now.AddMinutes(10);
        }
    }

    private static void AutoSave()
    {
        if (Application.isPlaying)
            return; // don’t autosave while in play mode
        Debug.Log("Starting autosave.");

        int sceneCount = SceneManager.sceneCount;
        int savedCount = 0;
        string savedScenes = "";

        for (int i = 0; i < sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.isDirty) // only save if modified
            {
                EditorSceneManager.SaveScene(scene);
                savedCount++;
                savedScenes += $"\n   • {scene.name}";
            }
        }

        AssetDatabase.SaveAssets();

        if (savedCount > 0)
            Debug.Log($"💾 AutoSave complete — {savedCount} scene(s) saved:{savedScenes}");
        else
            Debug.Log("💾 AutoSave skipped — no modified scenes to save.");
    }
}
