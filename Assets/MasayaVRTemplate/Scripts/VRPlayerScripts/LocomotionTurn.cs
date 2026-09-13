using UnityEngine;

public class LocomotionTurn : MonoBehaviour
{

    VRRig vrRig;
    [SerializeField] Transform head;
    [SerializeField] VRController controller;
    [SerializeField] float turnAngle = 45f;
    bool turned;

    private void Start()
    {
        vrRig = GetComponent<VRRig>();
    }

    private void Update()
    {
        float analogInputs = controller.controllerValues.analogValue.x;
        if (Mathf.Abs(analogInputs) < 0.2f)
            turned = false;

        if (turned)
            return;

        if(analogInputs >= 0.8f)
        {
            Turn(turnAngle);
        }
        else if(analogInputs <= -0.8f)
        {
            Turn(-turnAngle);
        }
    }

    void Turn(float turnValue)
    {
        vrRig.Turn();
        turned = true;

        Vector3 pivot = head.position;
        pivot.y = transform.position.y;

        transform.RotateAround(pivot, Vector3.up, turnValue);
    }

}
