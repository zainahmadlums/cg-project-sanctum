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

        // ✅ Smooth zoom with scroll input
        if (Mathf.Abs(zoomInput) > 0.01f)
        {
            targetDistance -= zoomInput * zoomSpeed * Time.deltaTime * 100f;
            targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        }

        distance = Mathf.Lerp(distance, targetDistance, Time.deltaTime * 8f);

        // ✅ Rotation (only while right mouse held)
        if (Mouse.current.rightButton.isPressed)
        {
            currentX += lookInput.x * rotationSpeed;
            currentY -= lookInput.y * rotationSpeed;
            currentY = Mathf.Clamp(currentY, minY, maxY);
        }

        Quaternion rot = Quaternion.Euler(currentY, currentX, 0);
        Vector3 desiredPos = target.position + rot * new Vector3(0, height, -distance);

        // ✅ Camera collision (to prevent clipping)
        if (Physics.Linecast(target.position + Vector3.up * 1.5f, desiredPos, out RaycastHit hit))
        {
            desiredPos = hit.point + hit.normal * 0.2f;
        }

        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothFollowSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
