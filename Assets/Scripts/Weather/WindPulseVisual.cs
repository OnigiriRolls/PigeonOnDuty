using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WindSource))]
public class WindPulseVisual : MonoBehaviour
{
    [SerializeField] private GameObject windRingPrefab;
    [SerializeField] private int ringsPerPulse = 3;
    [SerializeField] private float ringInterval = 0.15f;

    private WindSource windSource;

    private void Awake()
    {
        windSource = GetComponent<WindSource>();
    }

    private void OnEnable()
    {
        windSource.OnPulseStarted += HandlePulseStarted;
    }

    private void OnDisable()
    {
        windSource.OnPulseStarted -= HandlePulseStarted;
    }

    private void HandlePulseStarted()
    {
        StartCoroutine(SpawnRingsRoutine());
    }

    private IEnumerator SpawnRingsRoutine()
    {
        for (int i = 0; i < ringsPerPulse; i++)
        {
            SpawnRing();
            yield return new WaitForSeconds(ringInterval);
        }
    }

    private void SpawnRing()
    {
        Instantiate(windRingPrefab, transform.position, transform.rotation);
    }
}
