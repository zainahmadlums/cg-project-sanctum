using UnityEditor;
using UnityEngine;
using System.Reflection;

[InitializeOnLoad]
public static class PlayModeResetter
{
    static PlayModeResetter()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        // Runs right before Play starts
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            ResetStatics();
        }
    }

    private static void ResetStatics()
    {
        // Option 1: manually reset known statics
        //GameState.Reset();
        //PlayerInventory.Reset();
        // add more here if needed

        // Option 2: optional — brute-force clear ALL statics in your assemblies
        // Uncomment if you really want a total nuke (warning: can cause issues)
        /*
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsClass && type.IsAbstract && type.IsSealed) // static class
                {
                    foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if (field.FieldType.IsValueType)
                            field.SetValue(null, System.Activator.CreateInstance(field.FieldType));
                        else
                            field.SetValue(null, null);
                    }
                }
            }
        }
        */

        Debug.Log("✅ Static variables manually reset before Play Mode. Please add if you haven't.");
    }
}
