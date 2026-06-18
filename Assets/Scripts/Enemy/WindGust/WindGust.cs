using UnityEngine;

public class WindGust : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 25f;
    [SerializeField] private float lifeTime = 8f;
    [SerializeField] private float carryDuration = 1.5f;
    [SerializeField] private float upwardInfluence = 0.5f;
    [SerializeField] private float trackingDistance = 20f;

    private Transform player;
    private bool stoppedTracking;
    private Vector3 moveDirection;

    public void Initialize(Vector3 direction, Transform targetPlayer)
    {
        player = targetPlayer;
        moveDirection = direction.normalized;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (!stoppedTracking && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance > trackingDistance)
            {
                moveDirection = (player.position - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(moveDirection);
            }
            else
            {
                stoppedTracking = true;
            }
        }
        transform.position += moveSpeed * Time.deltaTime * moveDirection;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<PlayerController>(out var player))
            return;

        Vector3 carryDirection = (moveDirection + Vector3.up * upwardInfluence).normalized;
        player.StartWindCarry(carryDirection, carryDuration);
        Destroy(gameObject);
    }
}
