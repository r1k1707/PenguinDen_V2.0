using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NonVRMode : MonoBehaviour, IInputInteract, IInputGrab
{
    bool init;

    [SerializeField] Transform head;
    [SerializeField] GameObject debugUI;
    TextMeshProUGUI debugText;
    [SerializeField] Transform grabPosition;
    public Transform GrabPosition => grabPosition;
    [SerializeField] float movementSpeed;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float throwForce;
    ThrowValues throwValue = new ThrowValues();
    [SerializeField] float grabDistance;
    [SerializeField] LayerMask interactLayer;
    Rigidbody rb;

    VRActions inputs;
    Vector2 movement;
    Vector2 mouseInputs;
    float yaw;
    float pitch;
    bool focused;

    IInteractable interactObjList;
    IGrabbable grabObjList;

    IInteractable currentInteractObj;
    IGrabbable currentGrabObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        rb = GetComponent<Rigidbody>();
        debugUI.SetActive(true);
        debugText = debugUI.GetComponent<TextMeshProUGUI>();

        inputs = new VRActions();
        inputs.NonVRInputs.Interact.performed += Interact_performed;
        inputs.NonVRInputs.Interact.canceled += Interact_canceled;
        inputs.NonVRInputs.Grab.performed += Grab_performed;
        inputs.NonVRInputs.Grab.canceled += Grab_canceled;
        inputs.NonVRInputs.Throw.performed += Throw_performed;

        inputs.Enable();

        yield return null;
        init = true;
    }

    private void Throw_performed(InputAction.CallbackContext obj)
    {
        if(currentGrabObj != null)
        {
            throwValue.direction = head.forward;
            throwValue.force = throwForce;
            currentGrabObj.GrabEnd();
        }
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        if(currentInteractObj != null)
        {
            currentInteractObj.Interact();
        }
        else if(interactObjList != null)
        {
            currentInteractObj = interactObjList;
            currentInteractObj.InteractStart(this);
        }
    }
    private void Interact_canceled(InputAction.CallbackContext context)
    {
        if(currentInteractObj != null)
        {
            currentInteractObj.InteractEnd();
        }
    }
    private void Grab_performed(InputAction.CallbackContext context)
    {
        if (grabObjList != null)
        {
            currentGrabObj = grabObjList;
            currentGrabObj.GrabStart(this);
        }
    }
    private void Grab_canceled(InputAction.CallbackContext obj)
    {
        if(currentGrabObj != null)
        {
            currentGrabObj.GrabEnd();
        }
    }


    private void Update()
    {
        if (!init)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            focused = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            focused = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (!focused)
            return;

        mouseInputs = inputs.NonVRInputs.Camera.ReadValue<Vector2>();
        movement = inputs.NonVRInputs.Movement.ReadValue<Vector2>();
        UpdateCamera();
        CheckRaycast();
        UpdateUI();
    }

    private void FixedUpdate()
    {
        if (!focused)
            return;

        UpdateMovement();
    }

    void UpdateCamera()
    {
        yaw += mouseInputs.x * mouseSensitivity * Time.deltaTime;
        pitch -= mouseInputs.y * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -85f, 85f);

        transform.localEulerAngles = new Vector3(0, yaw, 0);
        head.localEulerAngles = new Vector3(pitch, 0, 0);
    }

    void UpdateMovement()
    {
        Vector3 forward = Vector3.ProjectOnPlane(head.forward, Vector3.up).normalized;
        Vector3 sideway = Vector3.ProjectOnPlane(head.right, Vector3.up).normalized;

        forward *= movement.y;
        sideway *= movement.x;

        Vector3 moveDirection = forward + sideway;
        rb.linearVelocity = moveDirection * movementSpeed * Time.deltaTime * 100;
    }

    private void OnDisable()
    {
        inputs.Disable();
    }

    void CheckRaycast()
    {
        RaycastHit hit;
        if(Physics.Raycast(head.position, head.forward, out hit, grabDistance, interactLayer))
        {
            if(hit.collider != null)
            {
                switch (hit.collider.tag)
                {
                    case "Interactable":
                        interactObjList = hit.collider.GetComponent<IInteractable>();
                        grabObjList = null;
                        break;
                    case "Grabbable":
                        grabObjList = hit.collider.GetComponent<IGrabbable>();
                        interactObjList = null;
                        break;
                    default:
                        grabObjList = null;
                        interactObjList = null;
                        break;
                }
            }
            else
            {
                grabObjList = null;
                interactObjList = null;
            }
        }
        else
        {
            grabObjList = null;
            interactObjList = null;
        }
    }

    void UpdateUI()
    {
        if (currentInteractObj != null)
        {
            debugText.text = $"Current Interaction - {((Component)currentInteractObj).gameObject.name}";
        }
        else if (currentGrabObj != null)
        {
            debugText.text = $"Currently Held - {((Component)currentGrabObj).gameObject.name}";
        }
        else if (grabObjList != null)
        {
            debugText.text = $"Grab Object Found - {((Component)grabObjList).gameObject.name}";
        }
        else if (interactObjList != null)
        {
            debugText.text = $"Interact Object Found - {((Component)interactObjList).gameObject.name}";
        }
        else
        {
            debugText.text = string.Empty;
        }
    }

    public void InteractFinish(bool removeFromList)
    {
        currentInteractObj = null;
    }

    public void GrabEnd()
    {
        currentGrabObj = null;
        throwValue.direction = Vector3.zero;
        throwValue.force = 0;
    }

    public void GrabGone(bool removeFromList, Transform obj)
    {
        currentGrabObj = null;
        throwValue.direction = Vector3.zero;
        throwValue.force = 0;
    }

    public ThrowValues GetThrowValues()
    {
        return throwValue;
    }
}
