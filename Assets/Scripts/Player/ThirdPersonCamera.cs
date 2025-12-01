using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;
    public float height = 2f;
    public float rotationSpeed = 3f;
    public float smoothFollowSpeed = 5f;

    public float minY = -20f;
    public float maxY = 60f;

    // Zoom settings
    public float minDistance = 2f;
    public float maxDistance = 10f;
    public float zoomSpeed = 2f;

    private float currentX;
    private float currentY;
    private float targetDistance;
    private PlayerInputActions input;

    [Header("Camera Shake")]
    public float shakeMagnitude = 0.3f;
    // We removed shakeDuration because you handle it via function calls, but keeping the var is fine.

    private float shakeTimer = 0f;

    void Awake()
    {
        input = new PlayerInputActions();
        targetDistance = distance;
    }

    void OnEnable() => input.Player.Enable();
    void OnDisable() => input.Player.Disable();

    void LateUpdate()
    {
        Vector2 lookInput = input.Player.Look.ReadValue<Vector2>();
        float zoomInput = input.Player.Zoom.ReadValue<float>();

        // 1. Handle Zoom
        if (Mathf.Abs(zoomInput) > 0.01f)
        {
            targetDistance -= zoomInput * zoomSpeed * Time.unscaledDeltaTime * 100f;
            targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        }
        distance = Mathf.Lerp(distance, targetDistance, Time.unscaledDeltaTime * 8f);

        // 2. Handle Input (Mouse Orbit)
        // Check if input is active (Game might be paused, but we want to process if allowed)
        if (Mouse.current.rightButton.isPressed)
        {
            currentX += lookInput.x * rotationSpeed;
            currentY -= lookInput.y * rotationSpeed;
            currentY = Mathf.Clamp(currentY, minY, maxY);
        }

        // 3. Calculate the "Clean" Goal (Where the camera WANTS to be smoothly)
        Quaternion cleanRot = Quaternion.Euler(currentY, currentX, 0);
        Vector3 cleanDesiredPos = target.position + cleanRot * new Vector3(0, height, -distance);

        // Handle Collision on the clean path
        if (Physics.Linecast(target.position + Vector3.up * 1.5f, cleanDesiredPos, out RaycastHit hit))
        {
            cleanDesiredPos = hit.point + hit.normal * 0.2f;
        }

        // 4. Smoothly move to the clean position (This is where your shake was dying before)
        Vector3 smoothedPos = Vector3.Lerp(transform.position, cleanDesiredPos, smoothFollowSpeed * Time.unscaledDeltaTime);

        // 5. CALCULATE SHAKE OFFSET
        // We calculate where the camera WOULD be if rotated by the shake amount, 
        // and add that difference to the final position.
        Vector3 shakeOffset = Vector3.zero;

        if (shakeTimer > 0)
        {
            float shakeX = (Random.value - 0.5f) * 2f * shakeMagnitude * 20f; // Multiplier to make 0.4 visible
            float shakeY = (Random.value - 0.5f) * 2f * shakeMagnitude * 20f;
            
            shakeTimer -= Time.unscaledDeltaTime;

            // Calculate "Shaken" rotation vs "Clean" rotation
            Quaternion shakenRot = Quaternion.Euler(currentY + shakeY, currentX + shakeX, 0);
            
            // Calculate where the camera would be relative to target with this shake
            Vector3 posWithShake = target.position + shakenRot * new Vector3(0, height, -distance);
            Vector3 posNoShake = target.position + cleanRot * new Vector3(0, height, -distance);

            shakeOffset = posWithShake - posNoShake;
            Debug.Log("heyooo");
        }

        // 6. Apply Final Position (Smooth Path + Raw Shake)
        transform.position = smoothedPos + shakeOffset;
        
        // 7. Look At Target
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        shakeTimer = duration;
        shakeMagnitude = magnitude;
    }
}