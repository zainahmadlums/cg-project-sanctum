using UnityEngine;

public class DoorController : Mechanism
{
    [Header("Door Settings")]
    public float openHeight = 3f;
    public float speed = 2f;

    [Tooltip("The vertical scale (Z) the door should have when fully open.")]
    public float targetScaleZ = 0.1f; // <--- ADD THIS LINE

    private Vector3 closedPos;
    private Vector3 openPos;
    private float closedScaleZ; // <--- ADD THIS LINE

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + Vector3.up * openHeight;
        
        closedScaleZ = transform.localScale.z; // <--- ADDED: Store the door's starting Y scale
        
        // Initial check in case requiredTriggers is 0
        if (requiredTriggers == 0) HandleStateChange(true);
    }

    void Update()
    {
        // Inherited 'isActive' tells us where to go
        Vector3 targetPos = isActive ? openPos : closedPos;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
        
        // --- ADDED: SCALING LOGIC ---
        float targetz = isActive ? targetScaleZ : closedScaleZ;
        Vector3 currentScale = transform.localScale;
        
        // Interpolate the Y scale towards the target scale (targetScalez or closedScalez)
        currentScale.z = Mathf.Lerp(currentScale.z, targetz, Time.deltaTime * speed);
        transform.localScale = currentScale;
        // ----------------------------
    }

    // This is called automatically by the base class
    protected override void HandleStateChange(bool active)
    {
        // If you needed to play a sound effect specific to the door, do it here.
        // The actual movement is handled in Update based on the boolean.
    }
}