using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro; // Needed for the text color change

public class MenuButtonVisuals : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Visual Settings")]
    public float scaleAmount = 1.1f;
    public float animationSpeed = 10f;
    
    [Header("Colors")]
    public Color normalColor = new Color(0f, 0.08f, 0.15f, 0.8f);
    public Color hoverColor = new Color(0f, 1f, 1f, 0.2f);
    
    public Color normalBorder = new Color(0f, 1f, 1f, 0.5f); 
    public Color hoverBorder = Color.cyan;

    public Color textColor = Color.white; // Explicitly requested white text

    private Vector3 _targetScale;
    private Color _targetColor;
    private Color _targetBorderColor;

    private Image _bgImage;
    private UnityEngine.UI.Outline _outline; 
    private TMP_Text _textComponent; // Reference to the text
    private Vector3 _originalScale;

    void Start()
    {
        _bgImage = GetComponent<Image>();
        _outline = GetComponent<UnityEngine.UI.Outline>();
        _textComponent = GetComponentInChildren<TMP_Text>(); // Find the text
        _originalScale = transform.localScale;

        if (_bgImage == null) _bgImage = gameObject.AddComponent<Image>();
        
        if (_outline == null) 
        {
            _outline = gameObject.AddComponent<UnityEngine.UI.Outline>();
            _outline.effectDistance = new Vector2(2, -2);
        }

        ResetVisuals();
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, Time.deltaTime * animationSpeed);
        
        if (_bgImage)
            _bgImage.color = Color.Lerp(_bgImage.color, _targetColor, Time.deltaTime * animationSpeed);
        
        if (_outline)
            _outline.effectColor = Color.Lerp(_outline.effectColor, _targetBorderColor, Time.deltaTime * animationSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _targetScale = _originalScale * scaleAmount;
        _targetColor = hoverColor;
        _targetBorderColor = hoverBorder;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetVisuals();
    }

    private void ResetVisuals()
    {
        _targetScale = _originalScale;
        _targetColor = normalColor;
        _targetBorderColor = normalBorder;

        // Force text color to white (or whatever is set in inspector)
        if (_textComponent != null)
            _textComponent.color = textColor;
    }
}