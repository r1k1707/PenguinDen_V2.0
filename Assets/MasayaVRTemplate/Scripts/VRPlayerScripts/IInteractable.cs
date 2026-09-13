using UnityEngine;

public interface IInteractable
{
    public void InteractStart(IInputInteract controller);
    public void Interact();
    public void InteractEnd();
}
