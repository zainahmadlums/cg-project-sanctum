using UnityEngine;

public interface ILightReactive
{
    void OnLightHit(RaycastHit hit, Vector3 incomingDir, Color beamColor, float beamThickness, GameObject source);
    void OnLightStopHit(GameObject source);
}
