using UnityEngine;
using UnityEngine.Events;

public abstract class Mechanism : MonoBehaviour
{
    [Tooltip("How many separate triggers must be active simultaneously?")]
    public int requiredTriggers = 1;

    [Header("Debug")]
    [SerializeField] protected int activeTriggers = 0;
    [SerializeField] protected bool isActive = false;

    // Optional: UnityEvents allow you to drag-and-drop ANYTHING in the inspector
    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;

    // Accepts optional source for puzzle logic
    public virtual void AddTrigger(GameObject source = null)
    {
        activeTriggers++;
        CheckState();
    }

    // Accepts optional source for puzzle logic
    public virtual void RemoveTrigger(GameObject source = null)
    {
        activeTriggers = Mathf.Max(0, activeTriggers - 1);
        CheckState();
    }

    private void CheckState()
    {
        bool shouldBeActive = activeTriggers >= requiredTriggers;

        if (shouldBeActive != isActive)
        {
            isActive = shouldBeActive;
            HandleStateChange(isActive);

            if (isActive) OnActivate.Invoke();
            else OnDeactivate.Invoke();
        }
    }

    // Force children to implement this.
    protected abstract void HandleStateChange(bool active);
}