using System.Collections;
using UnityEngine;

public class CrowController : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Patrol")]
    public Vector3 patrolCenter;
    public Vector3 patrolBoxSize = new(30f, 0f, 30f);
    // public float patrolSpeed = 5f;

    [Header("Detection")]
    public float detectRange = 20f;

    [Header("Chase")]
    //public float chaseSpeed = 30f;
    // public float followDistance = 10f;
    public float tolerance = 0.5f;

    [Header("Dash")]
    // public float dashSpeed = 18f;
    public float dashDistance = 10f;
    public int maxAttacks = 3;

    [Header("Recover")]
    public float recoverTime = 3f;

    [Header("Movement")]
    public float moveSpeed = 10f;
    public float rotationSpeed = 5f;

    [Header("Follow")]
    public float followDistance = 4f;
    public float deadZone = 1f;

    [Header("Target Update")]
    public float targetUpdateDelay = 0.1f;

    [Header("Speeds")]
    public float patrolSpeed = 6f;
    public float chaseSpeed = 12f;
    public float dashSpeed = 30f;

    [Header("Debug")]
    public int currentAttacks = 0;
    public Vector3 currentTarget;

    public Vector3 movementTarget;

    private Vector3 lastTargetPosition;

    private Rigidbody rb;

    private bool isUpdatingTarget;

    private Vector3 currentVelocity;

    private CrowState currentState;
    private float currentMoveSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>().transform;
        patrolCenter = transform.position;
        ChangeState(new CrowPatrolState(this));
    }

    private void Update()
    {
        currentState?.UpdateState();
    }

    private void FixedUpdate()
    {
        MoveCrow();
    }

    public void ChangeState(CrowState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public bool PlayerInRange()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        return dist <= detectRange;
    }

    public Vector3 GetRandomPatrolPoint()
    {
        float randomX = Random.Range(-patrolBoxSize.x / 2f, patrolBoxSize.x / 2f);
        float randomZ = Random.Range(-patrolBoxSize.z / 2f, patrolBoxSize.z / 2f);
        Vector3 randomOffset = new Vector3(randomX, 0f, randomZ);
        return patrolCenter + randomOffset;
    }

    //public void MoveTowards(Vector3 target, float speed)
    //{
    //    Vector3 toTarget = target - transform.position;
    //    float dist = toTarget.magnitude;

    //    transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

    //    if (dist <= 1f) return;
    //    Vector3 dir = (target - transform.position).normalized;
    //    if (dir == Vector3.zero) return;

    //    Quaternion targetRotation = Quaternion.LookRotation(dir);
    //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 3f * Time.deltaTime);
    //}

    public void SetMovementTarget(Vector3 target)
    {
        lastTargetPosition = target;
    }

    public void SetMoveSpeed(float speed)
    {
        currentMoveSpeed = speed;
    }

    public void UpdateDelayedTarget(Vector3 target)
    {
        if (!isUpdatingTarget)
        {
            StartCoroutine(UpdateTargetCoroutine(target));
        }
    }

    private IEnumerator UpdateTargetCoroutine(Vector3 target)
    {
        isUpdatingTarget = true;
        yield return new WaitForSeconds(targetUpdateDelay);
        lastTargetPosition = target;
        isUpdatingTarget = false;
    }

    private void MoveCrow()
    {
        Vector3 direction = lastTargetPosition - transform.position;
        float distance = direction.magnitude;
        if (distance < deadZone)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 moveDirection = direction.normalized;
        currentVelocity = moveDirection * currentMoveSpeed;
        rb.linearVelocity = currentVelocity;
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    public void MoveSmoothlyTo(Vector3 target, float speed)
    {
        Vector3 newPosition = Vector3.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);
        Vector3 direction = (target - transform.position).normalized;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    public void DashTowards(Vector3 target, float dashSpeed)
    {
        Vector3 dir = (target - transform.position).normalized;
        rb.linearVelocity = dir * dashSpeed;
        Quaternion targetRotation = Quaternion.LookRotation(dir);
        rb.rotation = targetRotation;
    }

    public void StopMovement()
    {
        rb.linearVelocity = Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(patrolCenter, patrolBoxSize);
    }
}
