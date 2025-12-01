using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Outline))]
public class WeightObject : MonoBehaviour, IInteractable
{
    [Header("Physics Settings")]
    public float holdSpeed = 15f;       
    public float rotationSpeed = 10f;   
    public float breakDistance = 2.5f;  

    private Rigidbody rb;
    private Outline outline;
    private Collider col;
    private Transform currentHoldPoint;
    
    private int originalLayer;
    private bool isHeld = false;
    private float holdOffsetZ;
    private Vector3 originalScale;

    // --- NEW INTERFACE IMPLEMENTATION ---
    public bool IsHeld => isHeld;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        outline = GetComponent<Outline>();
        col = GetComponent<Collider>();

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        outline.enabled = false;
        
        originalLayer = gameObject.layer;
        originalScale = transform.localScale;
    }

    public void OnHoverEnter()
    {
        if (!isHeld) outline.enabled = true;
    }

    public void OnHoverExit()
    {
        outline.enabled = false;
    }

    public void OnInteract(Transform holdPoint)
    {
        if (isHeld) Drop();
        else Pickup(holdPoint);
    }

    private void Pickup(Transform holdPoint)
    {
        if (isHeld) return;

        isHeld = true;
        currentHoldPoint = holdPoint;
        outline.enabled = false;

        // Calculate offset
        holdOffsetZ = col.bounds.extents.z + 0.1f;

        // Physics Setup
        rb.useGravity = false; 
        rb.linearDamping = 10f; 
        rb.angularDamping = 10f;
        rb.isKinematic = false;

        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Player"));
    }

    public void Drop()
    {
        if (!isHeld) return;

        isHeld = false;
        currentHoldPoint = null;

        // Reset Physics
        rb.useGravity = true;
        rb.linearDamping = 0f; 
        rb.angularDamping = 0.05f;

        // Reset Scale (Just in case physics squashed it)
        transform.localScale = originalScale;

        SetLayerRecursively(gameObject, originalLayer);

        rb.AddForce(transform.forward * 2f, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        if (isHeld && currentHoldPoint != null)
        {
            MoveToTarget();
        }
    }

    private void MoveToTarget()
    {
        Vector3 targetPos = currentHoldPoint.position + (currentHoldPoint.forward * holdOffsetZ);

        // Check Break Distance
        float distance = Vector3.Distance(transform.position, targetPos);
        if (distance > breakDistance)
        {
            Drop(); // This is what causes the desync!
            return;
        }

        // Physics Move
        Vector3 direction = targetPos - transform.position;
        rb.linearVelocity = direction * holdSpeed;

        // Physics Rotate
        Quaternion targetRot = currentHoldPoint.rotation;
        Quaternion nextRot = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(nextRot);
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}