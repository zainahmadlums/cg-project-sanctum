using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    [Header("Settings")]
    public float interactionRadius = 2f;
    public LayerMask interactableLayer;
    public Transform holdPoint; 

    private PlayerInputActions input;
    private IInteractable currentHover;
    private IInteractable currentlyHolding; 

    void Awake()
    {
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Player.Enable();
        input.Player.Interact.performed += OnInteractInput;
    }

    void OnDisable()
    {
        input.Player.Interact.performed -= OnInteractInput;
        input.Player.Disable();
    }

    void Update()
    {
        // --- SYNC FIX START ---
        // If we think we are holding something, but that thing says "I'm not held"
        // (because it hit a wall and auto-dropped), then clear our reference.
        if (currentlyHolding != null && !currentlyHolding.IsHeld)
        {
            currentlyHolding = null;
        }
        // --- SYNC FIX END ---

        if (currentlyHolding != null) 
        {
            if (currentHover != null)
            {
                currentHover.OnHoverExit();
                currentHover = null;
            }
            return;
        }

        ScanForInteractables();
    }

    private void ScanForInteractables()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRadius, interactableLayer);
        
        IInteractable closest = null;
        float closestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            var interactable = hit.GetComponent<IInteractable>();
            if (interactable == null) continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closestDist)
            {
                closest = interactable;
                closestDist = dist;
            }
        }

        if (closest != currentHover)
        {
            if (currentHover != null) currentHover.OnHoverExit();
            currentHover = closest;
            if (currentHover != null) currentHover.OnHoverEnter();
        }
    }

    private void OnInteractInput(InputAction.CallbackContext ctx)
    {
        // Case 1: Drop what we are holding
        if (currentlyHolding != null)
        {
            currentlyHolding.OnInteract(null); 
            currentlyHolding = null;
            return;
        }

        // Case 2: Pick up what we are looking at
        if (currentHover != null)
        {
            currentlyHolding = currentHover;
            currentlyHolding.OnInteract(holdPoint);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}