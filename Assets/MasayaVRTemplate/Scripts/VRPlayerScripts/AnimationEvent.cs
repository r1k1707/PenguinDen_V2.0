using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : MonoBehaviour
{
    public UnityEvent animationEvent;

    public void PlayAnimationEvent()
    {
        animationEvent.Invoke();
    }
}
