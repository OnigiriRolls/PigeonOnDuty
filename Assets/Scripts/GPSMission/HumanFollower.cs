using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(NPCDialogueUI))]
public class HumanFollower : MonoBehaviour
{
    public float DistractionPercent => distractionMeter / maxDistraction;
    public float DistractionReactionTime => distractionReactionTime;
    public float RecallDuration => recallDuration;
    public float ParadeDuration => paradeDuration;
    public float CurrentSpeed { get; private set; }
    public float StateProgress { get; set; }
    public string StateLabel { get; set; }

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
    [SerializeField] private float recallDistance = 10f;
    [SerializeField] private float distractionReactionTime = 5f;
    [SerializeField] private float recallDuration = 2f;
    [SerializeField] private float paradeDuration = 10f;
    [SerializeField] private GameObject recallCircle;
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TMP_Text hintText;

    private Transform target;
    private Rigidbody rb;
    private CapsuleCollider capsule;
    private bool isMoving = true;
    private float distractionMeter;
    private NPCDialogueUI dialogueUI;
    private SpriteRenderer recallCircleRenderer;
    private IGPSHumanState currentState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        dialogueUI = GetComponent<NPCDialogueUI>();
        recallCircleRenderer = recallCircle.GetComponent<SpriteRenderer>();
        distractionMeter = maxDistraction;
        ChangeState(new FollowingState(this));
    }

    public void Initialize(Transform targetToFollow)
    {
        target = targetToFollow;
    }

    private void Update()
    {
        currentState?.Update();
    }

    private void FixedUpdate()
    {
        UpdateDistraction();
        currentState?.FixedUpdate();
    }

    public void ChangeState(IGPSHumanState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public bool FollowTarget()
    {
        if (target == null)
        {
            CurrentSpeed = 0f;
            return false;
        }
        Vector3 targetPosition = target.position;
        targetPosition.y = transform.position.y;
        Vector3 direction = targetPosition - transform.position;
        float distance = direction.magnitude;

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
            return true;
        }

        Vector3 moveDirection = direction.normalized;
        Vector3 nextPosition = rb.position + moveSpeed * Time.fixedDeltaTime * moveDirection;

        if (IsBlocked(nextPosition))
        {
            CurrentSpeed = 0f;
            return false;
        }

        rb.MovePosition(nextPosition);
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        CurrentSpeed = moveSpeed;

        return false;
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
    }

    public bool IsDistracted()
    {
        return distractionMeter <= 0f;
    }

    public void StopMoving()
    {
        CurrentSpeed = 0f;
    }

    public void RefreshDistractionMeter()
    {
        distractionMeter = maxDistraction;
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

    public float GetDistractionPercent()
    {
        return distractionMeter / maxDistraction;
    }

    public void ShowMessage(string message)
    {
        dialogueUI.ShowMessage(message);
    }

    public void ShowInteractionCircle(Color color)
    {
        recallCircleRenderer.color = color;
        recallCircle.SetActive(true);
    }

    public void HideInteractionCircle()
    {
        recallCircle.SetActive(false);
    }

    public void ShowHint(string message)
    {
        if (hintPanel.activeSelf && hintText.text == message)
            return;
        hintText.text = message;
        hintPanel.SetActive(true);
    }

    public void HideHint()
    {
        hintPanel.SetActive(false);
    }

    public bool CanRecallNow()
    {
        if (target == null)
            return false;
        float distance = Vector3.Distance(target.position, transform.position);
        return distance <= recallDistance;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, recallDistance);
    }
}
