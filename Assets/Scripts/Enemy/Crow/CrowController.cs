using UnityEngine;

public class CrowController : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Chase")]
    public float chaseSpeed = 30f;
    public float followTolerance = 1f;
    public float leftOffset = 1.5f;
    public float minWaitTime = 2f;
    public float maxWaitTime = 4f;

    [Header("Dash")]
    public float dashSpeed = 18f;
    public int maxAttacks = 3;

    [Header("Recover")]
    public float recoverTime = 3f;

    [Header("Debug")]
    public int currentAttacks = 0;
    public Vector3 currentTarget;
    public float rotationSpeed = 7f;

    public bool CanHitPlayer { get; set; }

    [SerializeField] private GameObject hitEffectPrefab;

    private CrowState currentState;
    private PlayerController playerController;
    private CrowManager manager;
    private Vector3 spawnPosition;

    public void Initialize(Transform playerTransform, CrowManager crowManager, Vector3 spawnPosition)
    {
        manager = crowManager;
        player = playerTransform;
        playerController = player.GetComponent<PlayerController>();
        this.spawnPosition = spawnPosition;
        ChangeState(new CrowChaseState(this));
    }

    private void Start()
    {

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

    public void MoveTowards(Vector3 target, float speed)
    {
        Vector3 dir = (target - transform.position).normalized;
        if (dir == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.SetPositionAndRotation(
            Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime),
            Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime)
        );
    }

    public void MoveTowardsSpawnPositionAndDestroyCrow(float speed)
    {
        MoveTowards(spawnPosition, speed);
        float distance = Vector3.Distance(transform.position, spawnPosition);
        if (distance < 10f)
        {
            manager.CrowFinished();
            Destroy(gameObject);
        }
    }

    public Vector3 GetPredictedPlayerPositionWithOffset(float predictionTime)
    {
        Quaternion yawOnly = Quaternion.Euler(0f, player.eulerAngles.y, 0f);
        Vector3 flatRight = yawOnly * Vector3.right;
        return player.position - flatRight * leftOffset + playerController.Velocity * predictionTime;
    }

    public Vector3 GetPredictedPlayerPositionWithOffset1(float predictionTime)
    {
        Quaternion yawOnly = Quaternion.Euler(0f, player.eulerAngles.y, 0f);
        Vector3 flatRight = yawOnly * Vector3.right;
        return player.position + flatRight * 5f + playerController.Velocity * predictionTime;
    }


    public Vector3 GetPredictedPlayerPosition(float predictionTime)
    {
        return player.position + playerController.Velocity * predictionTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!CanHitPlayer) return;
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(1);
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    public void DestroyCrow()
    {
        manager.CrowFinished();
        Destroy(gameObject);
    }
}
