using UnityEngine;
using UnityEngine.Events;

public class DefaultPlayerProximity : MonoBehaviour
{

    public UnityEvent proximityEventEnter;
    public UnityEvent proximityEventExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            proximityEventEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            proximityEventExit.Invoke();
        }
    }
}
