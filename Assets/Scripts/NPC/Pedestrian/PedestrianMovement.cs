using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PedestrianMovement : MonoBehaviour, INPCMovement
{
    public float CurrentSpeed => agent.velocity.magnitude;
    public PedestrianWaypoint CurrentWaypoint => currentWaypoint;

    [SerializeField] private WaypointNetwork waypointNetwork;
    [SerializeField] private PedestrianWaypoint currentWaypoint;
    [SerializeField] private bool success;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Initialize(WaypointNetwork network)
    {
        waypointNetwork = network;
    }

    public void MoveTo(Vector3 destination)
    {
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
}
