using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HumanFollower))]
public class NPCAnimationController : MonoBehaviour
{
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private Animator animator;
    private HumanFollower follower;
    private float previuosSpeed;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        follower = GetComponent<HumanFollower>();
    }

    private void Update()
    {
        float speed = follower.CurrentSpeed;
        if (Mathf.Approximately(speed, previuosSpeed))
            return;
        animator.SetFloat(SpeedHash, speed);
        previuosSpeed = speed;
    }
}
