using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DeathEffects : MonoBehaviour
{
    public static DeathEffects Instance;

    public CanvasGroup canvasGroup;
    public Image flashImage;
    public RectTransform screenRoot;
    public ThirdPersonCamera cameraRef;

    public float blackoutTime = 0.3f;

    void Awake()
    {
        Instance = this;
        canvasGroup.alpha = 0;
    }

    public void PlayDeathEffect(System.Action onComplete)
    {
        DOTween.KillAll(true);

        Sequence seq = DOTween.Sequence().SetUpdate(true);

        void FlashInstant(Color col)
        {
            flashImage.color = col;
            canvasGroup.alpha = 1;
        }

        float shakeDur = 0.04f;
        float flashHold = 0.04f;

        // FIXED HELPER SHAKE
        Tween Shake(float dur)
        {
            // Create the UI shake tween
            Tween t = screenRoot.DOShakeAnchorPos(dur, 22f, 40, 90f, false);
            
            // Hook into the OnPlay event. 
            // This ensures the camera shake only triggers when this tween actually Starts.
            t.OnPlay(() => {
                 if (cameraRef) cameraRef.ShakeCamera(dur, 0.5f);
            });

            return t;
        }

        // Pre-made transparent colors
        Color w = new Color(1, 1, 1, 0.5f);
        Color g = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        Color c = new Color(0, 0, 0, 0f);
        Color b = new Color(0, 0, 0, 1f);

        // =============== SEQUENCE ===============

        seq.AppendCallback(() => FlashInstant(w));
        seq.Join(Shake(shakeDur));
        seq.AppendInterval(flashHold);

        seq.AppendCallback(() => FlashInstant(g));
        seq.Join(Shake(shakeDur));
        seq.AppendInterval(flashHold);

        seq.AppendCallback(() => FlashInstant(w));
        seq.Join(Shake(shakeDur));
        seq.AppendInterval(flashHold);

        seq.AppendCallback(() => FlashInstant(new Color(0,0,0,0.5f)));
        seq.Join(Shake(shakeDur));
        seq.AppendInterval(flashHold);

        seq.AppendCallback(() => FlashInstant(w));
        seq.Join(Shake(shakeDur));
        seq.AppendInterval(flashHold);

        seq.AppendCallback(() => FlashInstant(c)); 
        canvasGroup.alpha = 0;
        seq.Join(Shake(shakeDur));
        seq.AppendInterval(flashHold);

        seq.AppendCallback(() => FlashInstant(w));
        seq.Join(Shake(shakeDur));
        seq.AppendInterval(flashHold);

        // FINAL BLACK
        seq.AppendCallback(() => FlashInstant(b));
        seq.AppendInterval(blackoutTime);

        seq.OnComplete(() => onComplete?.Invoke());
    }
}