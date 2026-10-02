using UnityEngine;

public class PenguinAnimations : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayLoseAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("isLost");
        }
        else
        {
            Debug.LogWarning("Animator missing on " + name);
        }
    }
}