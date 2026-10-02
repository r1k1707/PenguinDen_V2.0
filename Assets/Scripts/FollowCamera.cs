using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private bool freezeXZAxis = true;
    [SerializeField] private Camera targetCamera;

    private void Start()
    {
        // Fall back to Main Camera if none assigned manually
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera == null)
        {
            Debug.LogError("FollowCamera: No camera assigned and no camera with 'MainCamera' tag found!", this);
        }
    }

    // Update runs after the Camera updates its position/rotation
    private void Update()
    {
        if (targetCamera == null) return;

        if (freezeXZAxis)
        { 
            Vector3 targetPosition = targetCamera.transform.position;
            targetPosition.y = transform.position.y; // Match height to lock X and Z tilt

            transform.LookAt(targetPosition);

            // Flip 180 degrees if the billboard appears backward (depends on sprite/mesh orientation)
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            // Match camera orientation completely
            transform.rotation = targetCamera.transform.rotation;
        }
    }
}