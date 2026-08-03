using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class GPSDogController : MonoBehaviour, IThrowTarget, IProjectileTarget
{
    public Transform AimPoint => transform;
    public float PatrolSpeed => config.patrolSpeed;
    public float ChaseSpeed => config.fetchSpeed;
    public PatrolZone PatrolZone { get; set; }
    public GPSDogPatrolState PatrolState { get; private set; }
    public GPSDogGuardState GuardState { get; private set; }
    public GPSDogScaredState ScaredState { get; private set; }
    public GPSDogChaseState ChaseState { get; private set; }
    public GPSDogState CurrentState { get; private set; }

    [SerializeField] private DogConfig config;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] barkClips;
    [SerializeField] private AudioClip[] scaredClips;
    [SerializeField] private GameObject targetRing;

    private Animator animator;
    private NavMeshAgent agent;
    private float currentSpeed;
    private bool isActive = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        PatrolState = new GPSDogPatrolState(this);
        GuardState = new GPSDogGuardState(this);
        ScaredState = new GPSDogScaredState(this);
        ChaseState = new GPSDogChaseState(this);
    }

    private void Start()
    {
        ChangeState(PatrolState);
    }

    private void Update()
    {
        if (!isActive)
            return;

        UpdateAnimationSpeed();
        CurrentState?.UpdateState();
    }

    public void ChangeState(GPSDogState state)
    {
        CurrentState?.Exit();
        CurrentState = state;
        CurrentState.Enter();
    }

    private void UpdateAnimationSpeed()
    {
        if (Mathf.Approximately(currentSpeed, agent.velocity.magnitude))
            return;
        currentSpeed = agent.velocity.magnitude;
        animator.SetFloat("Speed", currentSpeed);
    }

    public void MoveTowards(Vector3 target, float speed)
    {
        if (!agent.enabled || !agent.isOnNavMesh)
            return;
        agent.speed = speed;
        agent.SetDestination(target);
    }

    public void StopMoving()
    {
        if (!agent.enabled || !agent.isOnNavMesh)
            return;
        agent.ResetPath();
    }

    public bool HasReachedDestination(float tolerance = 0.3f)
    {
        return !agent.pathPending && agent.remainingDistance <= tolerance;
    }

    public void PlayBark()
    {
        AudioManager.Instance.PlayRandomSFX(barkClips, audioSource);
    }

    public void PlayScaredSound()
    {
        AudioManager.Instance.PlayRandomSFX(scaredClips, audioSource);
    }

    public bool OnHit(ThrowableData item)
    {
        if (item.itemName != "Feather")
            return false;
        if (CurrentState == ScaredState)
            return false;
        ChangeState(ScaredState);
        return true;
    }

    public bool CanBeHitBy(ThrowableData item)
    {
        return item.itemName == "Feather";
    }

    public void ShowTargetRing(bool show)
    {
        targetRing.SetActive(show);
    }

    public bool HasReachedHuman(HumanFollower human)
    {
        return Vector3.Distance(transform.position, human.transform.position) < 5f;
    }

    public void Deactivate()
    {
        isActive = false;
    }
}
