using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public RectTransform initialPanel;
    public RectTransform optionsPanel;
    public RectTransform levelSelectPanel;

    [Header("Options UI")]
    public TMP_Text difficultyText; 
    private string[] _difficulties = { "Easy", "Medium", "Hard" };
    private int _difficultyIndex = 1;

    [Header("Settings")]
    public float slideDuration = 0.5f;
    public Ease slideEase = Ease.InOutQuad;

    void Start()
    {
        UpdateDifficultyText();
        
        initialPanel.anchoredPosition = Vector2.zero;
        optionsPanel.anchoredPosition = new Vector2(1920, 0); 
        levelSelectPanel.anchoredPosition = new Vector2(1920, 0);
    }

    public void OpenOptions()
    {
        SlidePanel(initialPanel, new Vector2(-1920, 0)); 
        SlidePanel(optionsPanel, Vector2.zero);
    }

    public void CloseOptions()
    {
        SlidePanel(optionsPanel, new Vector2(1920, 0));
        SlidePanel(initialPanel, Vector2.zero);
    }

    public void OpenLevelSelector()
    {
        SlidePanel(initialPanel, new Vector2(-1920, 0));
        SlidePanel(levelSelectPanel, Vector2.zero);
    }

    public void CloseLevelSelector()
    {
        SlidePanel(levelSelectPanel, new Vector2(1920, 0));
        SlidePanel(initialPanel, Vector2.zero);
    }

    private void SlidePanel(RectTransform panel, Vector2 targetPos)
    {
        panel.DOAnchorPos(targetPos, slideDuration).SetEase(slideEase);
    }

    public void ToggleDifficulty()
    {
        _difficultyIndex++;
        if (_difficultyIndex >= _difficulties.Length)
            _difficultyIndex = 0;

        UpdateDifficultyText();
    }

    private void UpdateDifficultyText()
    {
        if (difficultyText != null)
            difficultyText.text = _difficulties[_difficultyIndex];
    }

    public void PlayLevel(string levelName)
    {
        if (Application.CanStreamedLevelBeLoaded(levelName))
        {
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogError($"Scene '{levelName}' not found. Check Build Settings.");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}