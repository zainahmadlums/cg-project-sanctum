using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    public RectTransform healthBarRoot;
    public Image faceImage;

    [Header("Cogs")]
    public Image[] cogImages;
    public Sprite fullCog;
    public Sprite emptyCog;

    [Header("Face Sprites")]
    public Sprite faceHappy;
    public Sprite faceNormal;
    public Sprite faceSad;

    [Header("Settings")]
    public int maxHealth = 5;

    private void Start()
    {
        RefreshUI(maxHealth);
    }

    // Update visuals ONLY
    public void RefreshUI(int currentHealth)
    {
        for (int i = 0; i < cogImages.Length; i++)
            cogImages[i].sprite = (i < currentHealth) ? fullCog : emptyCog;

        // Face
        if (currentHealth == maxHealth)
            faceImage.sprite = faceHappy;
        else if (currentHealth > maxHealth / 2)
            faceImage.sprite = faceNormal;
        else
            faceImage.sprite = faceSad;
    }

    public void PlayDamageFX()
    {
        healthBarRoot.DOShakeAnchorPos(0.35f, 20f, 20, 90f, false)
            .SetUpdate(true);
    }

    public void PlayHealFX(int cogIndex)
    {
        if (cogIndex < 0 || cogIndex >= cogImages.Length) return;

        var cog = cogImages[cogIndex];
        cog.transform.localScale = Vector3.one;
        cog.transform.DOScale(1.3f, 0.12f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutQuad);
    }
}
