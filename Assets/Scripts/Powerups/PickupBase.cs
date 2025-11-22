using UnityEngine;

public abstract class PickupBase : MonoBehaviour
{
    [Header("Floating & Rotating")]
    public float rotateSpeed = 35f;
    public float floatAmplitude = 0.25f;
    public float floatFrequency = 2f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // Rotation
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);

        // Floating
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPickup(other.gameObject);
            Destroy(gameObject);
        }
    }

    // This is what child classes implement
    protected abstract void OnPickup(GameObject player);
}
