using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class DogController : MonoBehaviour, IThrowTarget
{
    public Transform AimPoint => transform;
    public PatrolZone PatrolZone { get; set; }
    public DogConfig Config => config;
    public DogState CurrentState { get; private set; }
    public DogPatrolState PatrolState { get; private set; }
    public DogFetchState FetchState { get; private set; }
    public DogCarryState CarryState { get; private set; }
    public DogScaredState ScaredState { get; private set; }
    public bool HasNewspaper => hasNewspaper;

    [SerializeField] private ThrowableData newspaper;
    [SerializeField] private DogConfig config;
    [SerializeField] private CarryVisual carryVisual;
    [SerializeField] private Sprite exclamationMark;
    [SerializeField] private GameObject targetRing;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] barkClips;
    [SerializeField] private AudioClip[] scaredClips;
    [SerializeField] private string state;

    private Animator animator;
    private bool hasNewspaper;
    private float barkTimer;
    private float nextBarkTime;
    private NavMeshAgent agent;
    private float currentSpeed;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        PatrolState = new DogPatrolState(this);
        FetchState = new DogFetchState(this);
        CarryState = new DogCarryState(this);
        ScaredState = new DogScaredState(this);
    }

    private void Start()
    {
        ChangeState(PatrolState);
        ResetBarkTimer();
    }

    private void ResetBarkTimer()
    {
        nextBarkTime = Random.Range(10f, 15f);
        barkTimer = 0f;
    }

    private void Update()
    {
        UpdateAnimationMoveSpeed();
        barkTimer += Time.deltaTime;
        if (CurrentState != ScaredState && barkTimer >= nextBarkTime)
        {
            AudioManager.Instance.PlayRandomSFX(barkClips, audioSource);
            ResetBarkTimer();
        }
        CurrentState?.UpdateState();
    }

    public void ChangeState(DogState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        state = newState.ToString();
        CurrentState.Enter();
    }

    private void UpdateAnimationMoveSpeed()
    {
        if (Mathf.Approximately(currentSpeed, agent.speed))
            return;
        currentSpeed = agent.speed;
        animator.SetFloat("Speed", currentSpeed);
    }

    public void MoveTowards(Vector3 target, float speed)
    {
        agent.speed = speed;
        agent.SetDestination(target);
    }

    public bool HasReachedDestination(float tolerance = 0.2f)
    {
        return !agent.pathPending && agent.remainingDistance <= tolerance;
    }

    public void PickUp()
    {
        hasNewspaper = true;
        carryVisual.Show(newspaper);
    }

    public void DropNewspaper()
    {
        if (!HasNewspaper)
            return;
        hasNewspaper = false;
        carryVisual.Hide();
        Instantiate(newspaper.pickupPrefab, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup") && CurrentState != CarryState)
        {
            Destroy(other.transform.parent.gameObject);
            ChangeState(CarryState);
            PickUp();
        }
    }

    public void ShowInterest()
    {
        carryVisual.Show(exclamationMark);
    }

    public void HideInterest()
    {
        carryVisual.Hide();
    }

    public void OnHit(ThrowableData item)
    {
        if (CurrentState == ScaredState)
            return;
        ChangeState(ScaredState);
    }

    public void ShowTargetRing(bool show)
    {
        targetRing.SetActive(show);
    }

    public void PlayScaredSound()
    {
        AudioManager.Instance.PlayRandomSFX(scaredClips, audioSource);
    }
}
