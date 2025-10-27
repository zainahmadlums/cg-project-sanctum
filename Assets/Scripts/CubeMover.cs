using UnityEngine;
using UnityEngine.InputSystem;

public class CubeMover : MonoBehaviour
{
    public Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            Debug.Log("Button pressed yay");
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            
        }
    }
}
