using UnityEngine;

[RequireComponent(typeof(Outline))]
public class LeverController : MonoBehaviour, IInteractable
{
    [Header("Mechanism Link")]
    public Mechanism targetMechanism;

    [Header("Visual Settings")]
    public Transform handle;
    public float inactiveY = 0.4f;
    public float activeY = -0.4f;
    public float speed = 5f;

    // CHANGED: Made public getter so Pattern can check state if needed
    public bool IsActivated { get; private set; } = false;
    
    private Outline outline;
    public bool IsHeld => false;

    void Awake()
    {
        outline = GetComponent<Outline>();
        if (outline)
        {
            outline.enabled = false;
            // outline.OutlineMode = Outline.Mode.OutlineAll;
            // outline.OutlineColor = Color.yellow;
            // outline.OutlineWidth = 5f;
        }
    }

    void Update()
    {
        float targetY = IsActivated ? activeY : inactiveY;
        // Simple move logic
        Vector3 targetPos = new Vector3(handle.localPosition.x, targetY, handle.localPosition.z);
        handle.localPosition = Vector3.Lerp(handle.localPosition, targetPos, Time.deltaTime * speed);
    }

    public void OnInteract(Transform holdPoint)
    {
        IsActivated = !IsActivated;

        // CHANGED: Pass 'gameObject' as the source
        if (targetMechanism)
        {
            if (IsActivated) targetMechanism.AddTrigger(gameObject);
            else targetMechanism.RemoveTrigger(gameObject);
        }
    }
    
    // NEW: Called by the puzzle logic when the player fails
    public void ForceReset()
    {
        if (!IsActivated) return; // Already off

        IsActivated = false;
        // We do NOT call targetMechanism.RemoveTrigger here because the 
        // Puzzle script handles its own logic reset.
    }

    public void OnHoverEnter()
    {
        if (outline) outline.enabled = true;
    }

    public void OnHoverExit()
    {
        if (outline) outline.enabled = false;
    }
}