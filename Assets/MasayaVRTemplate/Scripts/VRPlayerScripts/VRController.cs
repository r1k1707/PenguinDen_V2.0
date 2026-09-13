using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRController : MonoBehaviour
{
    bool initialised;
    bool linkingController;

    [SerializeField] ControllerHand controllerHand;

    [field: SerializeField]
    public Transform pointer { get; private set; }

    public VRController otherController { get; private set; }
    public VRControllerInteraction interaction { get; private set; }
    public VRControllerGrab grab { get; private set; }

    VRRig vrRig;

    UnityEngine.XR.InputDevice controller;

    public ControllerValues controllerValues { get; private set; }


    // Delegate Events
    public Action onTeleportStart;
    public Action onTeleportEnd;

    public Action onGrabStart;
    public Action onGrabEnd;

    public Action onInteractStart;
    public Action onInteractEnd;


    public enum ControllerHand
    {
        left,
        right
    }


    public ControllerHand GetHand()
    {
        return controllerHand;
    }


    public void Initialise(VRRig rig)
    {
        vrRig = rig;

        // Get opposite controller
        if (controllerHand == ControllerHand.left)
        {
            otherController =
                vrRig.GetController(VRRig.ControllerHand.Right);
        }
        else
        {
            otherController =
                vrRig.GetController(VRRig.ControllerHand.Left);
        }


        // Reference interaction scripts
        interaction = GetComponent<VRControllerInteraction>();
        grab = GetComponent<VRControllerGrab>();


        enabled = true;
        interaction.enabled = true;
        grab.enabled = true;


        if (!initialised && !linkingController)
        {
            StartCoroutine(LinkingController());
        }
    }


    IEnumerator LinkingController()
    {
        linkingController = true;

        if (controllerValues == null)
        {
            controllerValues = new ControllerValues();
        }


        while (!controller.isValid)
        {
            List<UnityEngine.XR.InputDevice> devices = new();


            InputDeviceCharacteristics characteristics =
                InputDeviceCharacteristics.Controller |
                InputDeviceCharacteristics.HeldInHand;


            switch (controllerHand)
            {
                case ControllerHand.left:

                    characteristics |=
                        InputDeviceCharacteristics.Left;

                    break;


                case ControllerHand.right:

                    characteristics |=
                        InputDeviceCharacteristics.Right;

                    break;
            }


            InputDevices.GetDevicesWithCharacteristics(
                characteristics,
                devices
            );


            if (devices.Count > 0)
            {
                controller = devices[0];
            }


            yield return null;
        }


        Debug.Log(
            $"{controllerHand} controller linked: {controller.name}"
        );


        ResetControllerValues();

        initialised = true;
        linkingController = false;
    }


    private void Update()
    {
        if (!initialised)
            return;


        // Controller disconnected
        if (!controller.isValid)
        {
            ResetControllerValues();

            initialised = false;


            if (!linkingController)
            {
                StartCoroutine(LinkingController());
            }


            return;
        }


        UpdateValues();
        UpdateButtons();
    }


    void UpdateValues()
    {
        // -------------------------
        // TRIGGER VALUE
        // -------------------------

        if (!controller.TryGetFeatureValue(
            CommonUsages.trigger,
            out float triggerValue))
        {
            triggerValue = 0f;
        }

        controllerValues.triggerValue = triggerValue;



        // -------------------------
        // GRIP VALUE
        // -------------------------

        if (!controller.TryGetFeatureValue(
            CommonUsages.grip,
            out float gripValue))
        {
            gripValue = 0f;
        }

        controllerValues.gripValue = gripValue;



        // -------------------------
        // ANALOG / THUMBSTICK
        // -------------------------

        if (controller.TryGetFeatureValue(
            CommonUsages.primary2DAxis,
            out Vector2 analogValue))
        {
            // Small deadzone to guarantee a clean zero
            if (analogValue.magnitude < 0.1f)
            {
                analogValue = Vector2.zero;
            }


            controllerValues.analogValue = analogValue;
        }
        else
        {
            // Important:
            // Stops old values getting stuck
            controllerValues.analogValue = Vector2.zero;
        }
    }


    void UpdateButtons()
    {
        // -------------------------
        // TRIGGER
        // -------------------------

        bool triggerPressed =
            controller.TryGetFeatureValue(
                CommonUsages.triggerButton,
                out bool trigger)
            && trigger;


        if (triggerPressed != controllerValues.triggerPressed)
        {
            controllerValues.triggerPressed = triggerPressed;


            if (triggerPressed)
            {
                OnTriggerPressed();
            }
            else
            {
                OnTriggerReleased();
            }
        }



        // -------------------------
        // GRIP
        // -------------------------

        bool gripPressed =
            controller.TryGetFeatureValue(
                CommonUsages.gripButton,
                out bool grip)
            && grip;


        if (gripPressed != controllerValues.gripPressed)
        {
            controllerValues.gripPressed = gripPressed;


            if (gripPressed)
            {
                OnGripPressed();
            }
            else
            {
                OnGripReleased();
            }
        }



        // -------------------------
        // PRIMARY BUTTON
        // -------------------------

        bool primaryPressed =
            controller.TryGetFeatureValue(
                CommonUsages.primaryButton,
                out bool primary)
            && primary;


        if (
            primaryPressed !=
            controllerValues.primaryButtonPressed
        )
        {
            controllerValues.primaryButtonPressed =
                primaryPressed;


            if (primaryPressed)
            {
                OnPrimaryPressed();
            }
            else
            {
                OnPrimaryReleased();
            }
        }



        // -------------------------
        // SECONDARY BUTTON
        // -------------------------

        bool secondaryPressed =
            controller.TryGetFeatureValue(
                CommonUsages.secondaryButton,
                out bool secondary)
            && secondary;


        if (
            secondaryPressed !=
            controllerValues.secondaryButtonPressed
        )
        {
            controllerValues.secondaryButtonPressed =
                secondaryPressed;


            if (secondaryPressed)
            {
                OnSecondaryPressed();
            }
            else
            {
                OnSecondaryReleased();
            }
        }



        // -------------------------
        // ANALOG / THUMBSTICK CLICK
        // -------------------------

        bool analogPressed =
            controller.TryGetFeatureValue(
                CommonUsages.primary2DAxisClick,
                out bool analog)
            && analog;


        if (analogPressed != controllerValues.analogPressed)
        {
            controllerValues.analogPressed =
                analogPressed;


            if (analogPressed)
            {
                OnAnalogPressed();
            }
            else
            {
                OnAnalogReleased();
            }
        }
    }


    void ResetControllerValues()
    {
        if (controllerValues == null)
            return;


        controllerValues.triggerPressed = false;
        controllerValues.triggerValue = 0f;

        controllerValues.gripPressed = false;
        controllerValues.gripValue = 0f;

        controllerValues.primaryButtonPressed = false;
        controllerValues.secondaryButtonPressed = false;

        controllerValues.analogPressed = false;
        controllerValues.analogValue = Vector2.zero;
    }


    private void OnDisable()
    {
        ResetControllerValues();
    }


    #region Individual Input Checks

    private void OnTriggerPressed()
    {
        // Trigger = Interact
        onInteractStart?.Invoke();
    }


    public void OnTriggerReleased()
    {
        onInteractEnd?.Invoke();
    }


    public void OnGripPressed()
    {
        // Grip = Grab
        onGrabStart?.Invoke();
    }


    public void OnGripReleased()
    {
        onGrabEnd?.Invoke();
    }


    private void OnPrimaryPressed()
    {

    }


    private void OnPrimaryReleased()
    {

    }


    private void OnSecondaryPressed()
    {

    }


    private void OnSecondaryReleased()
    {

    }


    public void OnAnalogPressed()
    {
        // Thumbstick Click = Teleport
        onTeleportStart?.Invoke();
    }


    public void OnAnalogReleased()
    {
        onTeleportEnd?.Invoke();
    }

    #endregion
}



[System.Serializable]
public class ControllerValues
{
    public bool triggerPressed;
    public float triggerValue;

    public bool gripPressed;
    public float gripValue;

    public bool primaryButtonPressed;
    public bool secondaryButtonPressed;

    public bool analogPressed;
    public Vector2 analogValue;
}