using UnityEngine;

public class WindRing : MonoBehaviour
{
    [SerializeField] private float expandSpeed = 10f;
    [SerializeField] private float lifeTime = 1f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.localScale += Vector3.one * expandSpeed * Time.deltaTime;
    }
}
