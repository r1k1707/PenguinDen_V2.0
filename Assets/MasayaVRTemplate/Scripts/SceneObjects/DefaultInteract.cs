using UnityEngine;
using UnityEngine.Events;

public class DefaultInteract : MonoBehaviour, IInteractable
{
    [SerializeField] bool canInteractAgain;
    public UnityEvent InteractEvent;

    public void InteractStart(IInputInteract controller)
    {
        if (!canInteractAgain)
        {
            gameObject.tag = "Untagged";
        }

        InteractEvent.Invoke();
        controller.InteractFinish(!canInteractAgain);
    }

    public void Interact()
    {

    }

    public void InteractEnd()
    {

    }



}
