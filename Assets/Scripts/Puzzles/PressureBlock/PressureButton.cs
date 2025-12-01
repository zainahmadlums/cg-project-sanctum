using UnityEngine;

public class PressureButton : MonoBehaviour
{
    // Now this can accept a Door, a Trap, a Lift, anything.
    public Mechanism targetMechanism;
    
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
            
            // Send signal to the brain
            if (targetMechanism) targetMechanism.AddTrigger();
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
                
                // Send signal to the brain
                if (targetMechanism) targetMechanism.RemoveTrigger();
            }
        }
    }
}