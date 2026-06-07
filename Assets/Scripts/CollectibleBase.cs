using System.Collections;
using UnityEngine;

public abstract class CollectibleBase : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float floatAmplitude = 0.25f;
    [SerializeField] private float floatFrequency = 2f;
    [SerializeField] private float lifetime = 20f;

    [Header("Effects")]
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private AudioClip collectClip;
    [SerializeField] private Animator animator;

    private Vector3 startPosition;

    protected virtual void Start()
    {
        startPosition = transform.position;
        StartCoroutine(DespawnRoutine());
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

    private IEnumerator DespawnRoutine()
    {
        yield return new WaitForSeconds(lifetime - 3f);
        animator.SetBool("Pulse", true);
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
