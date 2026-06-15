using System.Collections;
using UnityEngine;

public class WindAttackManager : MonoBehaviour
{
    [SerializeField] private WindGust windGustPrefab;
    [SerializeField] private Transform player;

    [Header("Attack")]
    [SerializeField] private float attackInterval = 20f;
    [SerializeField] private int minGusts = 2;
    [SerializeField] private int maxGusts = 5;
    [SerializeField] private float gustInterval = 1f;

    [Header("Spawn")]
    [SerializeField] private float spawnDistance = 80f;

    private bool attackRunning;

    private void Update()
    {
        if (attackRunning)
            return;
        attackInterval -= Time.deltaTime;
        if (attackInterval > 0f)
            return;
        StartCoroutine(WindAttackRoutine());
    }

    private IEnumerator WindAttackRoutine()
    {
        attackRunning = true;
        int gustCount = Random.Range(minGusts, maxGusts + 1);

        for (int i = 0; i < gustCount; i++)
        {
            SpawnRandomGust();
            yield return new WaitForSeconds(gustInterval);
        }
        attackInterval = Random.Range(3f, 8f);
        attackRunning = false;
    }

    private void SpawnRandomGust()
    {
        bool fromLeft = Random.value > 0.5f;
        Vector3 direction;
        if (fromLeft)
        {
            direction = player.right;
        }
        else
        {
            direction = -player.right;
        }

        Vector3 spawnPosition = player.position - direction * spawnDistance;
        WindGust gust = Instantiate(windGustPrefab, spawnPosition, Quaternion.LookRotation(direction));
        gust.Initialize(direction);
    }
}
