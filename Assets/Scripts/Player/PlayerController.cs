using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform cameraTransform;
    public float sprintMultiplier = 1.8f;
    private bool isSprinting;

    private Rigidbody rb;
    private bool isGrounded;
    private Vector2 moveInput;
    private PlayerInputActions input;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Player.Enable();
        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += _ => moveInput = Vector2.zero;
        input.Player.Jump.performed += _ => TryJump();
        input.Player.Sprint.performed += _ => isSprinting = true;
        input.Player.Sprint.canceled += _ => isSprinting = false;
    }

    void OnDisable()
    {
        input.Player.Disable();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (forward * moveInput.y + right * moveInput.x).normalized;
        float currentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;
        Vector3 targetVel = moveDir * currentSpeed;
        Vector3 velChange = targetVel - rb.linearVelocity;
        velChange.y = 0;

        rb.AddForce(velChange, ForceMode.VelocityChange);

        if (moveDir.magnitude > 0.1f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), 10f * Time.deltaTime);
    }

    void TryJump()
    {
        if (!isGrounded) return;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }
}
