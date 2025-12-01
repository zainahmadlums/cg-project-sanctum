using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class LevelButton : MonoBehaviour
{
    public int levelNumber;
    private MainMenuController _menuController;
    private Button _myButton;

    void Start()
    {
        _myButton = GetComponent<Button>();
        _menuController = FindObjectOfType<MainMenuController>();

        if (_menuController == null)
        {
            Debug.LogError("MainMenuController missing.");
            return;
        }

        TMP_Text btnText = GetComponentInChildren<TMP_Text>();
        if (btnText) 
        {
            btnText.text = levelNumber.ToString();
            btnText.enableAutoSizing = true; // Here is your requested laziness
        }

        _myButton.onClick.AddListener(OnClick);
        
        string sceneName = "Level" + levelNumber;
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            _myButton.interactable = false;
            var visuals = GetComponent<MenuButtonVisuals>();
            if(visuals) Destroy(visuals);
        }
    }

    void OnClick()
    {
        _menuController.PlayLevel("Level" + levelNumber);
    }
}