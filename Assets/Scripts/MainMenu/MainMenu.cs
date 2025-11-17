using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // load the next scene in Build Settings
        SceneManager.LoadScene("Character");
    }

    public void OpenOptions()
    {
        Debug.Log("Options Menu Opened!");
        // You can later open an Options panel here
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game!");
        Application.Quit();

        // Note: this won’t quit in the editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
