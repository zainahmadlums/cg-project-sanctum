using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class FreeCameraController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float fastMultiplier = 3f;

    [Header("Mouse Look")]
    public float lookSensitivity = 0.5f;
    public float minPitch = -89f;
    public float maxPitch = 89f;
    public bool invertX = false;
    public bool invertY = false;
    public bool cursorLock = false;

    private float yaw;
    private float pitch;
    private bool isRightMouseHeld;
    private float xMul;
    private float yMul;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        xMul = invertX ? -1 : 1;
        yMul = invertY ? -1 : 1;
        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void Update()
    {
        HandleMouse();
        HandleMovement();
    }

    void HandleMouse()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.rightButton.wasPressedThisFrame)
        {
            isRightMouseHeld = true;
            if (cursorLock)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        else if (mouse.rightButton.wasReleasedThisFrame)
        {
            isRightMouseHeld = false;
            if (cursorLock)
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }

        if (isRightMouseHeld)
        {
            Vector2 delta = mouse.delta.ReadValue() * lookSensitivity;
            yaw += delta.x * xMul;
            pitch -= delta.y * yMul;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        }
    }

    void HandleMovement()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        Vector3 dir = Vector3.zero;

        if (keyboard.wKey.isPressed) dir += transform.forward;
        if (keyboard.sKey.isPressed) dir -= transform.forward;
        if (keyboard.aKey.isPressed) dir -= transform.right;
        if (keyboard.dKey.isPressed) dir += transform.right;
        if (keyboard.eKey.isPressed) dir += transform.up;
        if (keyboard.qKey.isPressed) dir -= transform.up;

        float speed = moveSpeed;
        if (keyboard.leftShiftKey.isPressed) speed *= fastMultiplier;

        transform.position += dir * speed * Time.deltaTime;
    }
}
