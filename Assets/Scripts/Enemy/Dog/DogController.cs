using UnityEngine;

public class DogController : MonoBehaviour
{
    public PatrolZone PatrolZone { get; set; }
    public Transform Player => player;
    public DogConfig Config => config;
    public DogState CurrentState { get; private set; }
    public DogPatrolState PatrolState { get; private set; }

    [SerializeField] private Transform player;
    [SerializeField] private DogConfig config;

    private void Awake()
    {
        PatrolState = new DogPatrolState(this);
    }

    private void Start()
    {
        ChangeState(PatrolState);
    }

    private void Update()
    {
        CurrentState?.UpdateState();
    }

    public void ChangeState(DogState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.01f)
            return;
        direction.Normalize();
        transform.position += direction * speed * Time.deltaTime;
        transform.forward = direction;
    }
}
