using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class PedestrianMovement : MonoBehaviour, INPCMovement
{
    public float CurrentSpeed => agent.velocity.magnitude;
    public PedestrianWaypoint CurrentWaypoint => currentWaypoint;
    public event Action OnStuck;

    [SerializeField] private WaypointNetwork waypointNetwork;
    [SerializeField] private PedestrianWaypoint currentWaypoint;
    [SerializeField] private float stuckDistance = 0.2f;
    [SerializeField] private float stuckTime = 10f;
    [SerializeField] private bool success;

    private NavMeshAgent agent;
    private float stuckTimer;
    private Vector3 lastPosition;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = Random.Range(30, 71);
        agent.speed = Random.Range(10f, 20f);
    }

    public void Initialize(WaypointNetwork network)
    {
        waypointNetwork = network;
    }

    public void MoveTo(Vector3 destination)
    {
        if (!agent.enabled)
            return;
        if (!agent.isOnNavMesh)
            return;
        agent.isStopped = false;
        success = agent.SetDestination(destination);
    }

    public void Stop()
    {
        agent.isStopped = true;
    }

    public void Resume()
    {
        agent.isStopped = false;
    }

    public bool HasReachedDestination()
    {
        if (agent.pathPending)
            return false;
        if (agent.hasPath && agent.velocity.sqrMagnitude != 0f)
            return false;
        return true;
    }

    public bool HasReachedDestination(float acceptableDistance)
    {
        if (agent.pathPending)
            return false;
        if (agent.remainingDistance > acceptableDistance)
            return false;
        if (agent.hasPath && agent.velocity.sqrMagnitude != 0f)
            return false;
        return true;
    }

    public void MoveToNextWaypoint()
    {
        currentWaypoint = waypointNetwork.GetRandomWaypoint();
        if (currentWaypoint == null)
            return;
        MoveTo(currentWaypoint.transform.position);
    }

    private void Update()
    {
        CheckIfStuck();
    }

    private void CheckIfStuck()
    {
        if (!agent.hasPath || agent.isStopped)
        {
            stuckTimer = 0f;
            lastPosition = transform.position;
            return;
        }

        if (Vector3.Distance(transform.position, lastPosition) < stuckDistance)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckTime)
            {
                OnStuck?.Invoke();
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = transform.position;
    }
}
