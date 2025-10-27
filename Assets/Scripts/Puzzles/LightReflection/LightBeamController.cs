using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LightBeamController : MonoBehaviour
{
    [Header("Beam Properties")]
    public Transform origin;
    public Vector3 direction = Vector3.up;
    public float maxDistance = 50f;
    public LayerMask hitMask = ~0;

    [Header("Visual Properties")]
    public Color beamColor = Color.white;
    [Range(0.01f, 0.5f)] public float beamThickness = 0.05f;

    private LineRenderer line;

    // Last-known hit info
    private RaycastHit lastHit;
    private ILightReactive lastReceiver;
    private Transform lastHitTransform;
    private Vector3 lastHitTransformPos;
    private Quaternion lastHitTransformRot;
    private Vector3 lastHitPoint;
    private bool hadHitLastFrame = false;

    // Cached state to detect changes that require a visual update
    private Vector3 lastOriginPos;
    private Quaternion lastOriginRot;
    private Color lastColor;
    private float lastThickness;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        SetupLineRenderer();
    }

    private void SetupLineRenderer()
    {
        line.useWorldSpace = true;
        line.positionCount = 2;
        line.startWidth = beamThickness;
        line.endWidth = beamThickness;

        // create a simple unlit material instance so changes to color don't affect other renderers
        line.material = new Material(Shader.Find("Unlit/Color"));
        line.material.color = beamColor;
    }

    private void Update()
    {
        if (origin == null) return;

        // Raycast every frame (required) and update the line's positions.
        Vector3 start = origin.position;
        Vector3 dir = origin.TransformDirection(direction.normalized);

        bool hit = Physics.Raycast(start, dir, out RaycastHit hitInfo, maxDistance, hitMask);
        Vector3 endPoint = hit ? hitInfo.point : (start + dir * maxDistance);

        // Always update visual positions/width/color every frame
        if (!line.enabled) line.enabled = true;
        line.SetPosition(0, start);
        line.SetPosition(1, endPoint);
        line.startWidth = beamThickness;
        line.endWidth = beamThickness;

        // Only assign material color if it's changed to avoid unnecessary material property sets
        if (beamColor != lastColor)
            line.material.color = beamColor;

        // Decide whether to notify receivers:
        if (hit)
        {
            HandleHit(hitInfo, dir);
        }
        else
        {
            // No hit this frame
            if (hadHitLastFrame)
                ClearLastHit();
        }

        // Cache common values for next-frame change detection
        hadHitLastFrame = hit;
        lastOriginPos = origin.position;
        lastOriginRot = origin.rotation;
        lastColor = beamColor;
        lastThickness = beamThickness;
    }

    private void HandleHit(RaycastHit hitInfo, Vector3 dir)
    {
        ILightReactive reactive = hitInfo.collider.GetComponent<ILightReactive>();
        Transform currentHitTransform = hitInfo.collider != null ? hitInfo.collider.transform : null;
        Vector3 currentHitPoint = hitInfo.point;

        // Case A: We hit a different receiver than before
        if (reactive != lastReceiver)
        {
            // Tell the previous receiver it stopped being hit
            if (lastReceiver != null)
                lastReceiver.OnLightStopHit(gameObject);

            // Tell the new receiver it is hit now
            if (reactive != null)
                reactive.OnLightHit(hitInfo, dir, beamColor, beamThickness, gameObject);

            // Save new receiver and its transform state
            lastReceiver = reactive;
            lastHitTransform = currentHitTransform;
            lastHitTransformPos = currentHitTransform != null ? currentHitTransform.position : Vector3.zero;
            lastHitTransformRot = currentHitTransform != null ? currentHitTransform.rotation : Quaternion.identity;
            lastHitPoint = currentHitPoint;
        }
        else
        {
            // Case B: Same receiver as previous frame.
            // But we still need to inform the receiver if either:
            //  - the receiver's transform moved/rotated
            //  - the hit point changed (e.g., because origin moved or beam angle changed)
            bool transformChanged = false;
            if (lastHitTransform != null)
            {
                if (lastHitTransform.position != lastHitTransformPos || lastHitTransform.rotation != lastHitTransformRot)
                    transformChanged = true;
            }

            bool hitPointChanged = currentHitPoint != lastHitPoint;

            if (transformChanged || hitPointChanged)
            {
                reactive?.OnLightHit(hitInfo, dir, beamColor, beamThickness, gameObject);

                // Update saved transform/point
                if (lastHitTransform != null)
                {
                    lastHitTransformPos = lastHitTransform.position;
                    lastHitTransformRot = lastHitTransform.rotation;
                }
                lastHitPoint = currentHitPoint;
            }
            else
            {
                // No significant change — do nothing (avoids spamming receivers).
                // If you want continuous per-frame updates for e.g. "time-based" reactions,
                // you can uncomment the next line:
                // reactive?.OnLightHit(hitInfo, dir, beamColor, beamThickness, gameObject);
                if (beamColor != lastColor || beamThickness != lastThickness)
                    reactive?.OnLightHit(hitInfo, dir, beamColor, beamThickness, gameObject);
            }
        }

        lastHit = hitInfo;
    }

    private void ClearLastHit()
    {
        if (lastReceiver != null)
        {
            lastReceiver.OnLightStopHit(gameObject);
            lastReceiver = null;
        }

        lastHitTransform = null;
        lastHitPoint = Vector3.zero;
        hadHitLastFrame = false;

        if (line != null)
            line.enabled = false;
    }

    public void ForceRefresh()
    {
        // Public method to force an immediate raycast+update
        Update();
    }

    private void OnDisable()
    {
        ClearLastHit();
        if (line != null)
            line.enabled = false;
    }
}
