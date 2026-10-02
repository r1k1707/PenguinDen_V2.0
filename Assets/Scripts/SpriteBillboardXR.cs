using UnityEngine;

public class SpriteBillboardXR : MonoBehaviour
{
    public Transform mainCameraTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
