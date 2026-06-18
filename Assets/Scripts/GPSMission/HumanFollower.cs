using UnityEngine;

public enum HumanState
{
    Following,
    Distracted,
    Returning
}
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

[RequireComponent(typeof(NPCDialogueUI))]
public class HumanFollower : MonoBehaviour
{
    public HumanState State => state;
    public float DistractionPercent => distractionMeter / maxDistraction;
    public float CurrentSpeed { get; private set; }
    public bool CanBeRecalled => state == HumanState.Distracted;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float stopDistance = 5f;
    [SerializeField] private float resumeDistance = 7f;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask paradeZoneLayer;
    [SerializeField] private float maxDistraction = 100f;
    [SerializeField] private float distractionDecayRate = 25f;
    [SerializeField] private float distractionRecoveryRate = 10f;
    [SerializeField] private float paradeCheckRadius = 0.5f;

    private Transform target;
    private Rigidbody rb;
    private CapsuleCollider capsule;
    private bool isMoving = true;
    private float distractionMeter;
    private HumanState state;
    private NPCDialogueUI dialogueUI;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        dialogueUI = GetComponent<NPCDialogueUI>();
        distractionMeter = maxDistraction;
        state = HumanState.Following;
    }

    public void Initialize(Transform targetToFollow)
    {
        target = targetToFollow;
    }

    private void FixedUpdate()
    {
        UpdateDistraction();
        if (state == HumanState.Distracted)
        {
            CurrentSpeed = 0f;
            return;
        }
        if (target == null)
        {
            CurrentSpeed = 0f;
            return;
        }

        Vector3 targetPosition = target.position;
        targetPosition.y = transform.position.y;
        Vector3 direction = targetPosition - transform.position;
        float distance = direction.magnitude;
        if (state == HumanState.Returning)
        {
            if (distance <= stopDistance)
            {
                state = HumanState.Following;
                CurrentSpeed = 0f;
                return;
            }
        }
        if (isMoving)
        {
            if (distance <= stopDistance)
                isMoving = false;
        }
        else
        {
            if (distance >= resumeDistance)
                isMoving = true;
        }

        if (!isMoving)
        {
            CurrentSpeed = 0f;
            return;
        }

        Vector3 moveDirection = direction.normalized;
        Vector3 nextPosition = rb.position + moveSpeed * Time.fixedDeltaTime * moveDirection;

        bool blocked = IsBlocked(nextPosition);
        if (blocked)
        {
            CurrentSpeed = 0f;
            return;
        }

        rb.MovePosition(nextPosition);
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        CurrentSpeed = moveSpeed;
    }

    private void UpdateDistraction()
    {
        bool insideParade = Physics.CheckSphere(transform.position, paradeCheckRadius, paradeZoneLayer);
        if (insideParade)
        {
            distractionMeter -= distractionDecayRate * Time.fixedDeltaTime;
            Debug.DrawRay(transform.position, Vector3.forward * 5f, Color.red);
        }
        else
            distractionMeter += distractionRecoveryRate * Time.fixedDeltaTime;
        distractionMeter = Mathf.Clamp(distractionMeter, 0f, maxDistraction);

        if (distractionMeter <= 0f && state == HumanState.Following)
        {
            BecomeDistracted();
        }
    }

    private bool IsBlocked(Vector3 nextPosition)
    {
        Vector3 center = nextPosition + capsule.center;
        float radius = capsule.radius * 0.95f;
        float halfHeight = Mathf.Max(capsule.height * 0.5f - radius, 0f);
        Vector3 point1 = center + Vector3.up * halfHeight;
        Vector3 point2 = center - Vector3.up * halfHeight;
        return Physics.CheckCapsule(point1, point2, radius, obstacleLayer);
    }

    private void BecomeDistracted()
    {
        state = HumanState.Distracted;
        CurrentSpeed = 0f;
        ShowMessage("I'll join them.");
        Debug.Log("Human joined the parade");
    }

    public void Recall()
    {
        if (state != HumanState.Distracted)
            return;
        distractionMeter = maxDistraction;
        state = HumanState.Returning;
        ShowMessage("I lost you for a second...");
        Debug.Log("Human recalled");
    }

    public float GetDistractionPercent()
    {
        return distractionMeter / maxDistraction;
    }

    public void ShowMessage(string message)
    {
        dialogueUI.ShowMessage(message);
    }
}
