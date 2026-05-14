using UnityEngine;

public class CrowController : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Patrol")]
    public Vector3 patrolCenter;
    public Vector3 patrolBoxSize = new(30f, 0f, 30f);
    public float patrolSpeed = 5f;

    [Header("Detection")]
    public float detectRange = 20f;

    [Header("Chase")]
    public float chaseSpeed = 30f;
    public float followDistance = 10f;
    public float tolerance = 0.5f;

    [Header("Dash")]
    public float dashSpeed = 18f;
    public float dashDistance = 10f;
    public int maxAttacks = 3;

    [Header("Recover")]
    public float recoverTime = 3f;

    [Header("Debug")]
    public int currentAttacks = 0;
    public Vector3 currentTarget;

    private CrowState currentState;

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

    public void MoveTowards(Vector3 target, float speed)
    {
        Vector3 toTarget = target - transform.position;
        float dist = toTarget.magnitude;

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (dist <= 1f) return;
        Vector3 dir = (target - transform.position).normalized;
        if (dir == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 3f * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(patrolCenter, patrolBoxSize);
    }

    public Vector3 GetPredictedPlayerPosition(float predictionTime)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        return player.position + playerController.Velocity * predictionTime;
    }
}
