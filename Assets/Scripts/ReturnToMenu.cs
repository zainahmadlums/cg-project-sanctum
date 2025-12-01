using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Exact name of your menu scene.")]
    public string menuSceneName = "MainMenuScene";

    private void OnTriggerEnter(Collider other)
    {
        // Make sure your Player object actually has the tag "Player"
        // or this will ignore you like I want to.
        if (other.CompareTag("Player"))
        {
            // Unlock cursor so they can actually click menu buttons
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SceneManager.LoadScene(menuSceneName);
        }
    }
}