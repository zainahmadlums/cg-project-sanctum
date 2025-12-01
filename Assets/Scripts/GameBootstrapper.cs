using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameBootstrapper 
{
    // Add the names of scenes where you DO NOT want the player to spawn
    private static readonly string[] ExemptScenes = new string[] 
    { 
        "MainMenuScene", 
        "UI"
    };

    // The exact name of your prefab inside the Resources folder
    private const string SYSTEM_PREFAB_NAME = "CoreGameplay";

    // This attribute runs this method automatically when the game starts
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        // Hook into the scene change event so we run this logic every time a level loads
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // Run it manually for the first scene
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. Check if we are in a "No Spawn" scene
        foreach (string name in ExemptScenes)
        {
            if (scene.name == name) return;
        }

        // 2. Check if the system already exists (manual override)
        if (GameObject.Find(SYSTEM_PREFAB_NAME)) return;

        // --- NEW STEP 2.5: NUKE THE DEFAULT CAMERA ---
        // Find any object tagged "MainCamera" that is NOT part of our new system (which doesn't exist yet)
        // and destroy it. This gets rid of the default camera Unity adds to new scenes.
        GameObject existingCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (existingCamera != null)
        {
            // Optional: Only destroy it if it's at the root level (not a child of something important)
            Object.Destroy(existingCamera);
        }
        // ---------------------------------------------

        // 3. Load the prefab from Resources
        GameObject systemPrefab = Resources.Load<GameObject>(SYSTEM_PREFAB_NAME);

        if (systemPrefab == null)
        {
            Debug.LogError($"[Bootstrapper] Could not find prefab named '{SYSTEM_PREFAB_NAME}' in a Resources folder!");
            return;
        }

        // 4. Spawn it
        GameObject instance = Object.Instantiate(systemPrefab);
        instance.name = SYSTEM_PREFAB_NAME; 

        // 5. Position Player (Spawn Point Logic)
        GameObject spawnPoint = GameObject.Find("PlayerSpawnPoint");
        if (spawnPoint)
        {
            Transform player = instance.transform.Find("Player"); 
            if (player) 
            {
                // If using CharacterController, disable it before moving or it will fight you
                CharacterController cc = player.GetComponent<CharacterController>();
                if(cc) cc.enabled = false;

                player.position = spawnPoint.transform.position;
                player.rotation = spawnPoint.transform.rotation;

                if(cc) cc.enabled = true;
                
                Physics.SyncTransforms(); 
            }
        }
    }
}