using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRControllerGrab : MonoBehaviour, IInputGrab
{
    VRRig vrRig;
    VRController controller;
    List<Transform> grabbableList = new List<Transform>();
    public IGrabbable currentHeld { get; private set; }

    BoxCollider bc;

    List<PhysicsSample> samples = new List<PhysicsSample>();
    struct PhysicsSample
    {
        public float time;
        public Vector3 pos;
    }

    private IEnumerator Start()
    {
        yield return null;
        controller = GetComponent<VRController>();
        vrRig = VRRig.Instance;
        bc = GetComponent<BoxCollider>();
        controller.onGrabStart += GrabStart;
        controller.onGrabEnd += GrabCanceled;
    }

    private void LateUpdate()
    {
        CalculateForce();
    }

    private void OnDisable()
    {
        controller.onGrabStart -= GrabStart;
        controller.onGrabEnd -= GrabEnd;
    }

    public void GrabStart()
    {
        if(grabbableList.Count > 0)
        {
            currentHeld = grabbableList[0].GetComponent<IGrabbable>();
            currentHeld.GrabStart(this);
        }
    }

    public void GrabCanceled()
    {
        if (currentHeld != null)
        {
            currentHeld.GrabEnd();
        }
    }

    public void GrabEnd()
    {
        currentHeld = null;
        ColliderCheck();
    }

    void ColliderCheck()
    {
        Vector3 colPos = bc.center;
        Vector3 colSize = bc.size;
        Collider[] hits = Physics.OverlapBox(transform.position + colPos, colSize / 2, transform.rotation);
        if(hits.Length > 0)
        {
            foreach(Collider col in hits)
            {
                if (col.gameObject.CompareTag("Grabbable") && !grabbableList.Contains(col.transform))
                {
                    grabbableList.Add(col.transform);
                }
            }
        }
    }

    public void GrabGone(bool removeFromList, Transform obj)
    {
        if (removeFromList)
        {
            grabbableList.Remove(obj);
        }
        currentHeld = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grabbable"))
        {
            grabbableList.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Grabbable"))
        {
            grabbableList.Remove(other.transform);
            if (grabbableList.Count == 0)
                grabbableList.Clear();
        }
    }

    void CalculateForce()
    {
        float now = Time.time;

        samples.Add(new PhysicsSample
        {
            time = now,
            pos = transform.position
        });

        if (samples.Count >= 30)
        {
            samples.RemoveAt(0);
        }
    }

    public ThrowValues GetThrowValues()
    {
        ThrowValues result = new ThrowValues()
        {
            force = 0f,
            direction = Vector3.zero
        };

        if (samples == null || samples.Count < 3)
            return result;

        Vector3 velocitySum = Vector3.zero;
        float weightSum = 0f;

        Vector3 referenceDirection = Vector3.zero;
        bool foundDirection = false;

        for (int i = samples.Count - 1; i > 0; i--)
        {
            float dt = samples[i].time - samples[i - 1].time;
            if (dt <= 0f)
                continue;

            Vector3 velocity =
                (samples[i].pos - samples[i - 1].pos) / dt;

            float speed = velocity.magnitude;

            // Ignore tiny movements / tracking jitter
            if (speed < vrRig.MinThrowSpeed)
                continue;

            Vector3 direction = velocity.normalized;

            if (!foundDirection)
            {
                referenceDirection = direction;
                foundDirection = true;
            }
            else
            {
                float dot = Vector3.Dot(referenceDirection, direction);

                // Direction changed too much, stop using older samples
                if (dot < 0.7f)
                    break;
            }

            velocitySum += velocity;
            weightSum++;
        }

        if (weightSum <= 0f)
            return result;

        Vector3 averageVelocity = velocitySum / weightSum;

        result.direction = averageVelocity.normalized;
        result.force = Mathf.Clamp(
            averageVelocity.magnitude * vrRig.ThrowMultiplier,
            0f,
            vrRig.MaxThrowForce
        );

        return result;
    }
}

public class ThrowValues
{
    public float force;
    public Vector3 direction;
    public Vector3 Throw => force * direction;
}
