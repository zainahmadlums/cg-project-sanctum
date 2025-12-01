using UnityEngine;
using System.Collections.Generic;

public class LightReceiver : MonoBehaviour, ILightReactive
{
    [Header("Linked Mechanism")]
    public Mechanism targetMechanism; // Generic link

    private int activeBeams = 0;
    private HashSet<GameObject> activeBeamSources = new HashSet<GameObject>();

    private void Start()
    {
        // Auto-find any mechanism (Door, Fan, etc) in parent if null
        if (targetMechanism == null)
            targetMechanism = GetComponentInParent<Mechanism>();
    }

    public void OnLightHit(RaycastHit hit, Vector3 direction, Color beamColor, float beamThickness, GameObject source)
    {
        if (activeBeamSources.Contains(source)) return;
        
        activeBeamSources.Add(source);
        
        // Tell mechanism we have a hit
        if (activeBeams == 0)
            targetMechanism?.AddTrigger();

        activeBeams++;
    }

    public void OnLightStopHit(GameObject source)
    {
        if (!activeBeamSources.Remove(source)) return;

        activeBeams = Mathf.Max(0, activeBeams - 1);

        // Tell mechanism we lost the light
        if (activeBeams == 0)
            targetMechanism?.RemoveTrigger();
    }
    
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = activeBeams > 0 ? Color.cyan : Color.green;
        Vector3 pos = transform.position;
        Gizmos.DrawLine(pos, pos + transform.up);
        Gizmos.DrawSphere(pos + transform.up, 0.1f);
    }
#endif
}