using UnityEngine;

public abstract class CollectibleBase : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float floatAmplitude = 0.25f;
    [SerializeField] private float floatFrequency = 2f;

    [Header("Effects")]
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private AudioClip collectClip;

    private Vector3 startPosition;

    protected virtual void Start()
    {
        startPosition = transform.position;
    }

    protected virtual void Update()
    {
        Rotate();
        Float();
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    private void Float()
    {
        Vector3 pos = startPosition;
        pos.y += Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Collect(other.gameObject);
        PlayEffects();
        Destroy(gameObject);
    }

    protected abstract void Collect(GameObject player);

    private void PlayEffects()
    {
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }
        if (collectClip != null)
        {
            AudioManager.Instance.PlaySFX(collectClip);
        }
    }
}
