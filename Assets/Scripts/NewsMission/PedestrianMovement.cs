using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PedestrianMovement : MonoBehaviour, INPCMovement
{
    public float CurrentSpeed => agent.velocity.magnitude;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Vector3 destination)
    {
        agent.isStopped = false;
        agent.SetDestination(destination);
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
        return agent.remainingDistance <= agent.stoppingDistance;
    }

    public bool HasReachedDestination(float acceptableDistance)
    {
        if (agent.pathPending)
            return false;
        return agent.remainingDistance <= acceptableDistance;
    }
}
