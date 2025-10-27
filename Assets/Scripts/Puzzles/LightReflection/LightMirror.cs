using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class LightMirror : MonoBehaviour, ILightReactive
{
    [Header("Reflective Faces")]
    public bool reflectUp = true;
    public bool reflectDown = false;
    public bool reflectLeft = true;
    public bool reflectRight = true;
    public bool reflectForward = false;
    public bool reflectBack = false;

    [Header("Beam Settings")]
    public float maxDistance = 50f;
    public LayerMask obstacleMask = ~0;
    public Color beamColor = Color.white;
    [Range(0.01f, 0.5f)] public float beamThickness = 0.05f;
    public bool useOwnBeamSettings = false;
    // Cached beam settings to detect changes
    private float lastMaxDistance;
    private Color lastBeamColor;
    private Color lastInBeamColor;
    private float lastBeamThickness;
    private float lastInBeamThickness;
    private bool lastUseOwnBeamSettings;
    private LayerMask lastObstacleMask;

    // 6 potential output beams
    private LightBeamController[] beams = new LightBeamController[6];

    // Tracks which sources are hitting this mirror
    private readonly Dictionary<GameObject, Vector3> incomingBeams = new Dictionary<GameObject, Vector3>();

    // Cache transform for performance
    private Vector3 lastPos;
    private Quaternion lastRot;

    private void Awake()
    {
        CreateBeamControllers();

        CacheSettings();
        lastInBeamColor = beamColor;
        lastInBeamThickness = beamThickness;
    }

    private void CreateBeamControllers()
    {
        // Create all 6 beam controllers (disabled by default)
        for (int i = 0; i < 6; i++)
        {
            GameObject beamObj = new GameObject($"MirrorBeam_{i}");
            beamObj.transform.SetParent(transform, false);
            var controller = beamObj.AddComponent<LightBeamController>();
            controller.origin = transform;
            controller.direction = FaceToDirection(i);
            controller.maxDistance = maxDistance;
            controller.hitMask = obstacleMask;
            controller.beamColor = beamColor;
            controller.beamThickness = beamThickness;
            beamObj.SetActive(false);
            beams[i] = controller;
        }

        CacheTransform();
    }

    private Vector3 FaceToDirection(int faceIndex)
    {
        return faceIndex switch
        {
            0 => Vector3.up,
            1 => Vector3.down,
            2 => Vector3.left,
            3 => Vector3.right,
            4 => Vector3.forward,
            5 => Vector3.back,
            _ => Vector3.up
        };
    }

    private bool FaceIsReflective(int faceIndex)
    {
        return faceIndex switch
        {
            0 => reflectUp,
            1 => reflectDown,
            2 => reflectLeft,
            3 => reflectRight,
            4 => reflectForward,
            5 => reflectBack,
            _ => false
        };
    }

    private int GetEntryFaceIndex(Vector3 incomingDir)
{
        Vector3 localDir = transform.InverseTransformDirection(-incomingDir.normalized);
        Vector3 halfSize = transform.localScale * 0.5f;

        // Avoid division by zero by using float.MaxValue for zero components
        float rx = (localDir.x != 0f) ? Mathf.Abs(halfSize.x / localDir.x) : float.MaxValue;
        float ry = (localDir.y != 0f) ? Mathf.Abs(halfSize.y / localDir.y) : float.MaxValue;
        float rz = (localDir.z != 0f) ? Mathf.Abs(halfSize.z / localDir.z) : float.MaxValue;

        // Determine the axis with the smallest ratio
        if (rx <= ry && rx <= rz) return (localDir.x > 0f) ? 3 : 2; // Right : Left
        if (ry <= rz)             return (localDir.y > 0f) ? 0 : 1; // Up : Down
        return (localDir.z > 0f) ? 4 : 5;                             // Forward : Back
    }

    public void OnLightHit(RaycastHit hit, Vector3 incomingDir, Color inColor, float inThickness, GameObject source)
    {
        //Vector3 localHitDir = transform.InverseTransformPoint(hit.point);
        int entryFace = GetEntryFaceIndex(transform.position - hit.point);
        Debug.Log(entryFace + " and is it reflective? " + FaceIsReflective(entryFace));

        // Skip if entry face is not reflective
        if (!FaceIsReflective(entryFace))
        {
            if (incomingBeams.ContainsKey(source))
            {
                incomingBeams.Remove(source);
                // If no sources remain, disable all beams
                if (incomingBeams.Count == 0)
                {
                    foreach (var b in beams)
                        if (b != null && b.gameObject.activeSelf)
                            b.gameObject.SetActive(false);
                }
            }
            return;
        }

        incomingBeams[source] = incomingDir;

        LightBeamController entryBeam = beams[entryFace];
        if (entryBeam != null && entryBeam.gameObject.activeSelf)
            entryBeam.gameObject.SetActive(false);

        // Reflect through all other reflective faces
        for (int i = 0; i < 6; i++)
        {
            if (i == entryFace) continue;
            if (!FaceIsReflective(i)) continue;

            LightBeamController beam = beams[i];
            beam.origin = transform;
            beam.direction = FaceToDirection(i);

            beam.maxDistance = maxDistance;
            beam.hitMask = obstacleMask;
            beam.beamColor = useOwnBeamSettings ? beamColor : inColor;
            beam.beamThickness = useOwnBeamSettings ? beamThickness : inThickness;

            if (!beam.gameObject.activeSelf)
                beam.gameObject.SetActive(true);

            beam.ForceRefresh();
        }
        lastInBeamColor = inColor;
        lastInBeamThickness = inThickness;
    }

    public void OnLightStopHit(GameObject source)
    {
        if (incomingBeams.ContainsKey(source))
            incomingBeams.Remove(source);

        // If no beams are hitting the mirror, hide all outgoing beams
        if (incomingBeams.Count == 0)
        {
            foreach (var beam in beams)
            {
                if (beam != null && beam.gameObject.activeSelf)
                    beam.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        bool transformChanged = transform.position != lastPos || transform.rotation != lastRot;
        bool settingsChanged = maxDistance != lastMaxDistance ||
                            beamColor != lastBeamColor ||
                            beamThickness != lastBeamThickness ||
                            useOwnBeamSettings != lastUseOwnBeamSettings ||
                            obstacleMask != lastObstacleMask;
        // Mirror moved or rotated → refresh all active beams
        if (transformChanged || settingsChanged)
        {
            foreach (var beam in beams)
            {
                if (beam != null && beam.gameObject.activeSelf)
                    if (settingsChanged)
                    {
                        beam.maxDistance = maxDistance;
                        beam.hitMask = obstacleMask;
                        beam.beamColor = useOwnBeamSettings ? beamColor : lastInBeamColor;
                        beam.beamThickness = useOwnBeamSettings ? beamThickness : lastInBeamThickness;
                    }
                    beam.ForceRefresh();
            }

            CacheTransform();
            if (settingsChanged)
            {
                CacheSettings();
            }
        }
    }

    private void CacheTransform()
    {
        lastPos = transform.position;
        lastRot = transform.rotation;
    }
    private void CacheSettings()
    {
        lastMaxDistance = maxDistance;
        lastBeamColor = beamColor;
        lastBeamThickness = beamThickness;
        lastUseOwnBeamSettings = useOwnBeamSettings;
        lastObstacleMask = obstacleMask;
    }

    private void OnDisable()
    {
        foreach (var beam in beams)
        {
            if (beam != null)
                beam.gameObject.SetActive(false);
        }
        incomingBeams.Clear();
    }

    private void OnDestroy()
    {
        foreach (var beam in beams)
        {
            if (beam != null)
                Destroy(beam.gameObject);
        }
    }
    
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = beamColor;
        Vector3 pos = transform.position;

        void DrawFace(Vector3 dir, bool active)
        {
            if (!active) return;
            float gizmoLength = 1f;
            Vector3 worldDir = transform.TransformDirection(dir);
            Vector3 offsetPos = pos + Vector3.Scale(transform.localScale / 2f, dir);
            Gizmos.DrawLine(offsetPos, offsetPos + worldDir * gizmoLength);
            Gizmos.DrawSphere(offsetPos + worldDir * gizmoLength, 0.07f);
        }

        DrawFace(Vector3.up, reflectUp);
        DrawFace(Vector3.down, reflectDown);
        DrawFace(Vector3.left, reflectLeft);
        DrawFace(Vector3.right, reflectRight);
        DrawFace(Vector3.forward, reflectForward);
        DrawFace(Vector3.back, reflectBack);
    }
#endif
}
