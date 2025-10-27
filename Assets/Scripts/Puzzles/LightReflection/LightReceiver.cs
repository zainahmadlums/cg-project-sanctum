using UnityEngine;
using System.Collections.Generic;


public class LightReceiver : MonoBehaviour, ILightReactive
{
    [Header("Linked Door")]
    public DoorController door;  // Assign in Inspector or auto-find in Start()

    private int activeBeams = 0;
    private HashSet<GameObject> activeBeamSources = new HashSet<GameObject>();


    private void Start()
    {
        // Try to auto-link a door if not assigned
        if (door == null)
            door = GetComponentInParent<DoorController>();
    }

    public void OnLightHit(RaycastHit hit, Vector3 direction, Color beamColor, float beamThickness, GameObject source)
    {
        Debug.Log(activeBeams);
        if (activeBeamSources.Contains(source))
        {
            return;
        }
        activeBeamSources.Add(source);
        // When first hit, increment beam count and activate door if necessary
        if (activeBeams == 0)
            door?.Activate();

        activeBeams++;
    }

    public void OnLightStopHit(GameObject source)
    {
        if (!activeBeamSources.Remove(source))
        {
            return;
        }

        // When a beam stops hitting, decrement count and close if none remain
        activeBeams = Mathf.Max(0, activeBeams - 1);

        if (activeBeams == 0)
            door?.Deactivate();
    }
    
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = activeBeams > 0 ? Color.cyan : Color.green;
        Vector3 pos = transform.position;
        float gizmoLength = 1f;
        Vector3 dir = transform.up * gizmoLength;

        Gizmos.DrawLine(pos, pos + dir);
        Gizmos.DrawSphere(pos + dir, gizmoLength / 10);
    }
#endif
}
