using UnityEngine;

public class WindGust : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 25f;
    [SerializeField] private float lifeTime = 8f;
    [SerializeField] private WindSource windSource;

    private Vector3 moveDirection;

    public void Initialize(Vector3 direction)
    {
        windSource.SetDirection(direction);
        moveDirection = direction.normalized;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += moveSpeed * Time.deltaTime * moveDirection;
    }
}
