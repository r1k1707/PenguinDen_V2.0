using System.Collections.Generic;
using UnityEngine;

public class ResetSpheres : MonoBehaviour
{
    public List<Transform> spheres = new List<Transform>();
    List<Vector3> originalPositions = new List<Vector3>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform t in spheres)
        {
            Vector3 pos = t.position;
            originalPositions.Add(pos);
        }
    }

    public void Reset()
    {
        int i = 0;

        foreach(Transform t in spheres)
        {
            t.parent = null;

            t.GetComponent<DefaultGrab>().ForceGrabEnd();
            Rigidbody rb = t.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.position = originalPositions[i];

            i++;
        }
    }
}
