using UnityEngine;

[DisallowMultipleComponent]
public class LightEmitter : MonoBehaviour
{
    [Header("Beam Settings")]
    public float maxDistance = 20f;
    public LayerMask obstacleMask;
    public Color beamColor = Color.white;
    [Range(0.01f, 0.5f)] public float beamThickness = 0.05f;
    public bool beamEnabled = true;

    private LightBeamController beamController;

    // Cached values to detect changes
    private Vector3 lastPos;
    private Quaternion lastRot;
    private float lastMaxDistance;
    private Color lastColor;
    private float lastThickness;
    private bool lastEnabled;

    private void Awake()
    {
        CreateBeamController();
        ApplyBeamSettings();
    }

    private void CreateBeamController()
    {
        // Create the beam as a child object
        GameObject beamObj = new GameObject("LightBeam");
        beamObj.transform.SetParent(transform, false);
        beamController = beamObj.AddComponent<LightBeamController>();
    }

    private void ApplyBeamSettings()
    {
        beamController.origin = transform;
        beamController.direction = Vector3.up;
        beamController.maxDistance = maxDistance;
        beamController.hitMask = obstacleMask;
        beamController.beamColor = beamColor;
        beamController.beamThickness = beamThickness;

        beamController.gameObject.SetActive(beamEnabled);

        // Cache current state
        lastPos = transform.position;
        lastRot = transform.rotation;
        lastMaxDistance = maxDistance;
        lastColor = beamColor;
        lastThickness = beamThickness;
        lastEnabled = beamEnabled;

        if (beamEnabled)
            beamController.ForceRefresh();
    }

    private void Update()
    {
        // Detect changes to reapply settings dynamically
        bool changed =
            transform.position != lastPos ||
            transform.rotation != lastRot ||
            maxDistance != lastMaxDistance ||
            beamColor != lastColor ||
            beamThickness != lastThickness ||
            beamEnabled != lastEnabled;

        if (changed)
            ApplyBeamSettings();
    }

    private void OnValidate()
    {
        // Live updates inside editor when tweaking values
        if (Application.isPlaying && beamController != null)
            ApplyBeamSettings();
    }

    private void OnDisable()
    {
        if (beamController != null)
            beamController.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (beamController != null)
            Destroy(beamController.gameObject);
    }
    
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 start = transform.position;
        Vector3 dir = transform.up * 1f;
        Gizmos.DrawLine(start, start + dir);
        Gizmos.DrawSphere(start + dir, 0.1f);
    }
#endif
}
