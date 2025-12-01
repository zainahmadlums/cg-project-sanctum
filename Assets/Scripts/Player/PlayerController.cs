using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

//these numbers all for capsule height 1.9, radius 0.5

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
    private bool resettable;
    private Vector2 moveInput;
    private PlayerInputActions input;
    private Animator anim;
    private CapsuleCollider col;
    public Transform model; // reference to the robot model
    private float knockbackTimer; // <--- ADD THIS

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        input = new PlayerInputActions();
        anim = model.GetComponent<Animator>();
        col = GetComponent<CapsuleCollider>();

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
        isGrounded = Physics.SphereCast(
            transform.position + Vector3.up * (0.1f - col.height / 2 + col.radius), // sphere origin
            col.radius * 0.8f,           // sphere radius
            Vector3.down,     // direction
            out RaycastHit hit,
            0.15f     // distance to cast
        );
        // isGrounded = Physics.Raycast(
        //     transform.position + Vector3.up * 0.05f,
        //     Vector3.down,
        //     (col.height * 0.5f) + 0.1f
        // );
        anim.SetBool("Jumping", !isGrounded);
        if (resettable && isGrounded)
        {
            anim.SetBool("JumpStarted", false);
            resettable = false;
        } else if (!isGrounded)
        {
            resettable = true;
        }
    }

    void Move()
    {
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.deltaTime;
            return; // Skip input movement while stunned
        }
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

        float speed = rb.linearVelocity.magnitude;
        anim.SetFloat("Speed", speed, 0.15f, Time.deltaTime);

        if (moveDir.magnitude > 0.1f)
            model.rotation = Quaternion.Slerp(model.rotation, Quaternion.LookRotation(moveDir), 10f * Time.deltaTime);
    }

    void TryJump()
    {
        if (!isGrounded) return;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        anim.SetBool("JumpStarted", true);
        // isGrounded = false;
        // anim.SetBool("Jumping", !isGrounded);
    }

    // void OnCollisionStay(Collision collision)
    // {
    //     isGrounded = true;
    //     anim.SetBool("Jumping", !isGrounded);
    // }
    public void ApplyKnockback(Vector3 force, float stunDuration)
    {
        knockbackTimer = stunDuration;
        rb.linearVelocity = Vector3.zero; // Reset current movement so the hit feels impactful
        rb.AddForce(force, ForceMode.Impulse);
    }

    
}
