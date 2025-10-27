using UnityEngine;

public class PressureButton : MonoBehaviour
{
    public DoorController connectedDoor;
    public float pressDepth = 0.1f;
    private Vector3 startPos;
    private int triggerCount = 0;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<WeightObject>())
        {
            triggerCount++;
            transform.localPosition = startPos - Vector3.up * pressDepth;
            connectedDoor.Activate();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<WeightObject>())
        {
            triggerCount--;
            if (triggerCount <= 0)
            {
                transform.localPosition = startPos;
                connectedDoor.Deactivate();
            }
        }
    }
}
