using UnityEngine;
using UnityEngine.InputSystem; // You need this namespace now

public class MenuCameraParallax : MonoBehaviour
{
    [Header("Settings")]
    public float maxRotationAngle = 5f;
    public float smoothSpeed = 5f;

    private Quaternion _initialRotation;

    void Start()
    {
        _initialRotation = transform.rotation;
    }

    void Update()
    {
        // Check if a mouse is actually connected so we don't crash
        if (Mouse.current == null) return;

        // 1. Get mouse position using the New Input System
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // 2. Normalize to -1 to 1 range
        float xNorm = (mousePos.x / Screen.width) * 2f - 1f;
        float yNorm = (mousePos.y / Screen.height) * 2f - 1f;

        // 3. Calculate target rotation
        float rotY = xNorm * maxRotationAngle;
        float rotX = -yNorm * maxRotationAngle;

        Quaternion targetRotation = _initialRotation * Quaternion.Euler(rotX, rotY, 0);

        // 4. Smoothly rotate
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
    }
}