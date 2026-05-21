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
    public float dashDistance = 10f;
    public int maxAttacks = 3;

    [Header("Recover")]
    public float recoverTime = 3f;

    [Header("Debug")]
    public int currentAttacks = 0;
    public Vector3 currentTarget;
    public float rotationSpeed = 7f;

    private CrowState currentState;
    private PlayerController playerController;
    private CrowManager manager;

    public void Initialize(Transform playerTransform, CrowManager crowManager)
    {
        manager = crowManager;
        player = playerTransform;
        playerController = player.GetComponent<PlayerController>();
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

    public Vector3 GetPredictedPlayerPositionWithOffset(float predictionTime)
    {
        Quaternion yawOnly = Quaternion.Euler(0f, player.eulerAngles.y, 0f);
        Vector3 flatRight = yawOnly * Vector3.right;
        return player.position - flatRight * leftOffset + playerController.Velocity * predictionTime;
    }

    public Vector3 GetPredictedPlayerPosition(float predictionTime)
    {
        return player.position + playerController.Velocity * predictionTime;
    }

    public void DestroyCrow()
    {
        manager.CrowFinished();
        Destroy(gameObject);
    }
}
