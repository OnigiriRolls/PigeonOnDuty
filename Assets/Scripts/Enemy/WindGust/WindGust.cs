using UnityEngine;

public class WindGust : MonoBehaviour
{
    public WindGustConfig Config => config;

    [SerializeField] private WindGustConfig config;

    private Transform player;
    private bool stoppedTracking;
    private Vector3 moveDirection;
    private float moveSpeed;

    public void Initialize(Vector3 direction, Transform targetPlayer, float speed)
    {
        player = targetPlayer;
        moveDirection = direction.normalized;
        moveSpeed = speed;
        Destroy(gameObject, config.lifeTime);
    }

    private void Update()
    {
        if (!stoppedTracking && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance > config.trackingDistance)
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

        Vector3 carryDirection = (moveDirection + Vector3.up * config.upwardInfluence).normalized;
        player.StartWindCarry(carryDirection, config.carryDuration, 35f, 4f);
        Destroy(gameObject);
    }
}
