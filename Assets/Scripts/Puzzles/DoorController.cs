using UnityEngine;

public class DoorController : MonoBehaviour
{
    public bool isOpen = false;
    public float openHeight = 3f;
    public float speed = 2f;

    private Vector3 closedPos;
    private Vector3 openPos;
    private int activeTriggers = 0;

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + Vector3.up * openHeight;
    }

    void Update()
    {
        Vector3 target = isOpen ? openPos : closedPos;
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * speed);
    }

    public void Activate()
    {
        activeTriggers++;
        isOpen = true;
    }

    public void Deactivate()
    {
        activeTriggers = Mathf.Max(0, activeTriggers - 1);
        if (activeTriggers == 0)
            isOpen = false;
    }
}
