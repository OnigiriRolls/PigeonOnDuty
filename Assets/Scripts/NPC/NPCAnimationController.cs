using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(INPCMovement))]
public class NPCAnimationController : MonoBehaviour
{
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private Animator animator;
    private float previuosSpeed;
    private INPCMovement movement;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<INPCMovement>();
    }

    private void Update()
    {
        float speed = movement.CurrentSpeed;
        if (Mathf.Approximately(speed, previuosSpeed))
            return;
        animator.SetFloat(SpeedHash, speed);
        previuosSpeed = speed;
    }
}
