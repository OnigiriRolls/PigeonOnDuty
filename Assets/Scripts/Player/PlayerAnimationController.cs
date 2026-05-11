using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController player;

    private bool previousFlyingState;

    private void Update()
    {
        animator.SetBool("IsGliding", player.IsGliding);
        animator.SetBool("IsFlying", player.IsFlying);

        if (!previousFlyingState && player.IsFlying)
        {
            animator.SetTrigger("Takeoff");
        }
        if (previousFlyingState && !player.IsFlying)
        {
            animator.SetTrigger("Landing");
        }

        previousFlyingState = player.IsFlying;
    }
}
