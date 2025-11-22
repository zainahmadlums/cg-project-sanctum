using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using DG.Tweening; // DOTween

public class PauseMenu : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float tweenDuration = 0.3f;

    private bool isPaused = false;

    void Start()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void Update()
    {
        //Debug.Log("hmm");
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("hii");
            if (isPaused) CloseMenu();
            else OpenMenu();
        }
    }

    public void OpenMenu()
    {
        isPaused = true;
        Time.timeScale = 0; // pause the game
        Debug.Log("Here we go");
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOFade(1, tweenDuration).SetUpdate(true);
    }

    public void CloseMenu()
    {
        Debug.Log("going back");
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOFade(0, tweenDuration).SetUpdate(true).OnComplete(() =>
        {
            isPaused = false;
            Time.timeScale = 1; // resume the game
        });
    }

    public void OnResumeButton()
    {
        CloseMenu();
    }

    public void OnQuitButton()
    {
        Time.timeScale = 1; // ensure game is unpaused before quitting
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // stop play mode
        #else
            Application.Quit(); // quit build
        #endif
    }
}
