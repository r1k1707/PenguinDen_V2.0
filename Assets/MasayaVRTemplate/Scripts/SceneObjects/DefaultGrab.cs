using UnityEngine;

public class DefaultGrab : MonoBehaviour, IGrabbable
{
    IInputGrab returnDevice;
    Rigidbody rb;
    Collider col;

    [SerializeField] bool throwable;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = rb.GetComponent<Collider>();
    }

    public void GrabStart(IInputGrab controller)
    {
        if(returnDevice != null && returnDevice != controller)
        {
            returnDevice.GrabEnd();
        }

        returnDevice = controller;
        if(controller is NonVRMode)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            col.isTrigger = true;

            NonVRMode nonVRMode = (NonVRMode)controller;
            transform.parent = nonVRMode.GrabPosition;
        }
        else
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            col.isTrigger = true;

            Transform device = ((Component)controller).transform;
            transform.parent = device;
        }
    }
    public void GrabEnd()
    {
        if (returnDevice == null)
        {
            Debug.Log("Huh");
            return;
        }

        ThrowValues throwValues = returnDevice.GetThrowValues();
        Vector3 throwForce = throwValues.Throw;

        returnDevice.GrabEnd();
        returnDevice = null;

        rb.useGravity = true;
        rb.isKinematic = false;
        col.isTrigger = false;

        transform.parent = null;

        if (throwable)
        {
            rb.AddForce(throwForce);
        }
    }

    public void ForceGrabEnd()
    {
        if (returnDevice != null)
        {
            returnDevice.GrabEnd();
            returnDevice = null;
        }
    }
}
