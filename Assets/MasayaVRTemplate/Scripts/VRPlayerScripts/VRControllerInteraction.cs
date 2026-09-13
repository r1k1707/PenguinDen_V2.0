using System.Collections.Generic;
using UnityEngine;

public class VRControllerInteraction : MonoBehaviour, IInputInteract
{
    VRController controller;
    List<IInteractable> interactionObjects = new List<IInteractable>();
    [SerializeField] List<GameObject> objList = new List<GameObject>();
    public IInteractable currentInteraction { get; private set; }

    private void Start()
    {
        controller = GetComponent<VRController>();
        controller.onInteractStart += InteractStart;
        controller.onInteractEnd += InteractEnd;
    }

    private void OnDisable()
    {
        controller.onInteractStart -= InteractStart;
        controller.onInteractEnd -= InteractEnd;
    }

    public void InteractStart()
    {
        if (currentInteraction != null)
        {
            if (!interactionObjects.Contains(currentInteraction))
            {
                currentInteraction = null;
                if (interactionObjects.Count != 0)
                {
                    currentInteraction = interactionObjects[0];
                    currentInteraction.InteractStart(this);
                }
            }
            else
            {
                currentInteraction.Interact();
            }
        }
        else
        {
            if (interactionObjects.Count != 0)
            {
                currentInteraction = interactionObjects[0];
                currentInteraction.InteractStart(this);
            }
        }
    }

    public void InteractEnd()
    {
        if (currentInteraction != null)
        {
            currentInteraction.InteractEnd();
        }
    }

    //Can be called by the interactable object or other controller
    public void InteractFinish(bool removeFromList)
    {
        if (removeFromList)
        {
            interactionObjects.Remove(currentInteraction);
        }
        currentInteraction = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            interactionObjects.Add(other.GetComponent<IInteractable>());
            objList.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            interactionObjects.Remove(other.GetComponent<IInteractable>());
            objList.Remove(other.gameObject);
            if (interactionObjects.Count == 0)
                interactionObjects.Clear();
        }
    }
}
