using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class LocomotionTeleport : MonoBehaviour
{
    VRRig vrRig;
    VRController leftController;
    VRController rightController;
    VRController currentController;

    [SerializeField] LayerMask teleportLayer;
    [SerializeField] VRRig.ControllerHand teleportHand;
    [SerializeField] GameObject teleportPrefab;
    GameObject teleportVisualiser;
    [SerializeField] GameObject pointer;
    LineRenderer lineRenderer;

    bool teleportPoint;

    private void Start()
    {
        vrRig = GetComponent<VRRig>();
        lineRenderer = pointer.GetComponent<LineRenderer>();
        pointer.SetActive(false);
        leftController = vrRig.GetController(VRRig.ControllerHand.Left);
        rightController = vrRig.GetController(VRRig.ControllerHand.Right);
        AssignTeleportButtons();
    }

    public void TeleportStart()
    {
        teleportPoint = true;
        StartCoroutine(TeleportAim());
    }

    IEnumerator TeleportAim()
    {
        if (teleportVisualiser == null)
        {
            teleportVisualiser = Instantiate(teleportPrefab);
        }

        pointer.SetActive(true);
        Transform teleportLocator = teleportVisualiser.transform;
        Transform hand = currentController.pointer;
        RaycastHit hit;
        bool canTeleport = false;
        teleportVisualiser.SetActive(false);

        while (teleportPoint)
        {
            lineRenderer.SetPosition(0, hand.position);

            if (Physics.Raycast(hand.position, hand.forward, out hit, 20, teleportLayer))
            {
                teleportVisualiser.SetActive(true);
                canTeleport = true;
                teleportLocator.position = hit.point;
                lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                lineRenderer.SetPosition(1, hand.position + (hand.forward * 20));
            }
            yield return null;
        }

        if (canTeleport)
        {
            Vector3 teleportDestination = teleportLocator.position;
            teleportDestination -= new Vector3(vrRig.head.localPosition.x, 0, vrRig.head.localPosition.z);
            vrRig.transform.position = teleportDestination;
            teleportVisualiser.SetActive(false);
        }

        pointer.SetActive(false);
    }

    public void TeleportEnd()
    {
        teleportPoint = false;
    }

    void AssignTeleportButtons()
    {
        currentController = teleportHand == VRRig.ControllerHand.Left ? leftController : rightController;
        
        currentController.onTeleportStart += TeleportStart;
        currentController.onTeleportEnd += TeleportEnd;
    }

    public void UnsubcribeButtons()
    {
        currentController.onTeleportStart -= TeleportStart;
        currentController.onTeleportEnd -= TeleportEnd;
    }
}
