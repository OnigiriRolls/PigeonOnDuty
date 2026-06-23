using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(ParadeChatter))]
public class NPCParadePerson : MonoBehaviour, INPCMovement
{
    public float CurrentSpeed => agent.velocity.magnitude;

    private NavMeshAgent agent;
    private Vector3 paradeCenter;
    private bool reachedDestination;
    private ParadeChatter chatter;
    private Vector3 spawnPosition;
    private bool returningHome;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        chatter = GetComponent<ParadeChatter>();
    }

    public void Initialize(Vector3 centerPoint, Vector3 spawnPosition)
    {
        this.spawnPosition = spawnPosition;
        float radius = Random.Range(2f, 10f);
        paradeCenter = centerPoint;
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        Vector3 destination = centerPoint + new Vector3(randomCircle.x, 0f, randomCircle.y);
        agent.SetDestination(destination);
    }

    private void Update()
    {
        if (returningHome)
        {
            if (!agent.pathPending && agent.remainingDistance <= 0.2f)
                Destroy(gameObject);
            return;
        }
        if (reachedDestination)
            return;
        if (agent.pathPending)
            return;
        if (agent.remainingDistance > agent.stoppingDistance)
            return;
        reachedDestination = true;
        agent.updateRotation = false;
        agent.isStopped = true;
        FaceDestinationCenter();
        chatter.StartChatter();
    }

    private void FaceDestinationCenter()
    {
        Vector3 direction = paradeCenter - transform.position;
        direction.y = 0;
        if (direction.sqrMagnitude < 0.01f)
            return;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void ReturnHome()
    {
        returningHome = true;
        agent.updateRotation = true;
        agent.isStopped = false;
        agent.SetDestination(spawnPosition);
    }
}
