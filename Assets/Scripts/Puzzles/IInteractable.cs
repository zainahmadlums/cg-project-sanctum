using UnityEngine;

public interface IInteractable
{
    // Property to check if the object thinks it's being held
    bool IsHeld { get; }

    void OnHoverEnter();
    void OnHoverExit();
    void OnInteract(Transform holdPoint);
}