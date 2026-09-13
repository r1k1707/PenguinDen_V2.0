using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;


public class VRRig : MonoBehaviour
{
    VRActions inputs;

    [SerializeField] Color outlineColor;
    public static VRRig Instance { get; private set; }
    [field: SerializeField] public Transform head { get; private set; }
    [SerializeField] VRController[] controllers;
    [SerializeField] Volume volume;
    Vignette vignette;
    float vignetteAmount;

    BoxCollider bc;
    Rigidbody rb;

    [Header("Throw Values")]
    [SerializeField] float throwMultiplier = 100f;
    public float ThrowMultiplier => throwMultiplier;

    [SerializeField] float minThrowSpeed = 0.15f;
    public float MinThrowSpeed => minThrowSpeed;

    [SerializeField] float maxThrowForce = 500f;
    public float MaxThrowForce => maxThrowForce;

    [SerializeField] float minThrowMovement = 0.005f;
    public float MinThrowMovement => minThrowMovement;

    public enum ControllerHand
    {
        Left, Right
    };

    public enum ControllerButton
    {
        Trigger, Grip, Thumbstick, Primary, Secondary
    }


    private void Start()
    {
        Instance = this;
        List<XRDisplaySubsystem> vrDisplays = new List<XRDisplaySubsystem>();
        SubsystemManager.GetSubsystems(vrDisplays);

        inputs = new VRActions();
        inputs.Enable();



        if (vrDisplays.Count == 0)
        {
            Debug.Log("No Headset Connected");
            GetComponent<NonVRMode>().enabled = true;
            controllers[0].gameObject.SetActive(false);
            controllers[1].gameObject.SetActive(false);
            GetComponent<LocomotionTeleport>().enabled = false;
            GetComponent<LocomotionSmooth>().enabled = false;
            GetComponent<LocomotionTurn>().enabled = false;
            this.enabled = false;
        }
        else
        {
            foreach (XRDisplaySubsystem display in vrDisplays)
            {
                Debug.Log("VR Headset Found");
            }

            controllers[0].Initialise(this);
            controllers[1].Initialise(this);

            volume.sharedProfile.TryGet(out vignette);
        }

        bc = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
    }

    private List<UnityEngine.XR.InputDevice> controllersXR = new();

    private void Update()
    {

        controllersXR.Clear();

        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(
            UnityEngine.XR.InputDeviceCharacteristics.Controller,
            controllersXR
        );

        foreach (UnityEngine.XR.InputDevice device in controllersXR)
        {
            Vector2 stick;

            bool success = device.TryGetFeatureValue(
                UnityEngine.XR.CommonUsages.primary2DAxis,
                out stick
            );

            if (success)
            {
                Debug.Log($"{device.name}: Stick = {stick}");
            }
        }

        UpdateCollider();
        UpdateVignette();
    }

    void UpdateCollider()
    {
        Vector3 headPos = head.localPosition;
        Vector3 colSize = new Vector3(0.1f, 0.1f, 0.1f);
        headPos.y /= 2;
        bc.center = headPos;
        colSize.y = head.localPosition.y;
        bc.size = colSize;
    }

    void UpdateVignette()
    {
        if (rb.linearVelocity.magnitude >= 0.1f)
        {
            vignetteAmount += Time.deltaTime * 2;
        }
        else
        {
            vignetteAmount -= Time.deltaTime;
        }

        vignetteAmount = Mathf.Clamp(vignetteAmount, 0, 1);
        vignette.intensity.value = vignetteAmount;
    }

    public void Turn()
    {
        vignetteAmount = 1;
        vignette.intensity.value = vignetteAmount;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = outlineColor;
        Vector3 rightForward = transform.position + transform.right + transform.forward;
        Vector3 rightBack = transform.position + transform.right - transform.forward;
        Vector3 leftForward = transform.position - transform.right + transform.forward;
        Vector3 leftBack = transform.position - transform.right - transform.forward;
        Gizmos.DrawLine(rightForward, rightBack);
        Gizmos.DrawLine(leftForward, leftBack);
        Gizmos.DrawLine(rightForward, leftForward);
        Gizmos.DrawLine(rightBack, leftBack);
    }

    public VRController GetController(ControllerHand hand)
    {
        if(hand == ControllerHand.Left)
        {
            return controllers[0];
        }
        else
        {
            return controllers[1];
        }
    }
}
