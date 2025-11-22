using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DeathEffects : MonoBehaviour
{
    public static DeathEffects Instance;

    public CanvasGroup canvasGroup;
    public RectTransform screenRoot;   // some root panel for shake
    public float blackoutTime = 0.5f;

    void Awake()
    {
        Instance = this;
        canvasGroup.alpha = 0;
    }

    public void PlayDeathEffect(System.Action onComplete)
    {
        // STOP any prior tween nonsense
        DOTween.Kill(canvasGroup);
        DOTween.Kill(screenRoot);

        Sequence seq = DOTween.Sequence().SetUpdate(true);

        // 1. violent screen shake (0.2s)
        seq.Append(screenRoot.DOShakeAnchorPos(0.2f, 35f, 60, 90f, false));

        // 2. flash white really fast (0.1s)
        seq.Join(canvasGroup.DOFade(1f, 0.1f));

        // 3. fade to black (instead of white) for dramatic effect
        seq.AppendCallback(() => canvasGroup.GetComponent<Image>().color = Color.black);

        // keep it black for ~0.5 sec
        seq.AppendInterval(blackoutTime);

        // when done → callback to reload scene
        seq.OnComplete(() => onComplete?.Invoke());
    }
}
