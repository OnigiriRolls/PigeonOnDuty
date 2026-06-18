using System.Collections;
using TMPro;
using UnityEngine;

public class WindAttackManager : MonoBehaviour
{
    [SerializeField] private WindGust windGustPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private float attackIntervalMin = 10f;
    [SerializeField] private float attackIntervalMax = 20f;
    [SerializeField] private int minGusts = 2;
    [SerializeField] private int maxGusts = 5;
    [SerializeField] private float gustInterval = 1f;
    [SerializeField] private AudioClip[] windAttackClips;
    [SerializeField] private float warningDuration = 3f;
    [SerializeField] private GameObject warningObject;
    [SerializeField] private TMP_Text warningTimerText;
    [SerializeField] private Transform spawnPosLeft;
    [SerializeField] private Transform spawnPosRight;

    private float attackInterval;
    private bool attackRunning;

    private void Start()
    {
        attackInterval = Random.Range(attackIntervalMin, attackIntervalMax);
    }

    private void Update()
    {
        if (attackRunning)
            return;
        attackInterval -= Time.deltaTime;
        if (attackInterval > 0f)
            return;
        StartCoroutine(WindAttackSequence());
    }

    private IEnumerator WindAttackSequence()
    {
        attackRunning = true;
        if (warningObject != null)
            warningObject.SetActive(true);

        float timer = warningDuration;
        while (timer > 0f)
        {
            warningTimerText.text = Mathf.CeilToInt(timer).ToString();
            timer -= Time.deltaTime;
            yield return null;
        }

        if (warningObject != null)
            warningObject.SetActive(false);

        AudioClip clip = windAttackClips[Random.Range(0, windAttackClips.Length)];
        AudioManager.Instance.PlayEnvironmentalLoop(clip);

        yield return StartCoroutine(WindAttackRoutine());
        attackInterval = Random.Range(attackIntervalMin, attackIntervalMax);
        attackRunning = false;
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
        attackInterval = Random.Range(attackIntervalMin, attackIntervalMax);
        attackRunning = false;
        AudioManager.Instance.StopEnvironmentalLoop();
    }

    private void SpawnRandomGust()
    {
        Vector3[] directions = {
            player.right, -player.right,
            (player.right + Vector3.up).normalized, (-player.right + Vector3.up).normalized,
            (player.right + Vector3.down).normalized, (-player.right + Vector3.down).normalized };

        Vector3 direction = directions[Random.Range(0, directions.Length)];
        bool comesFromRight = Vector3.Dot(direction, player.right) > 0f;
        Vector3 spawnPosition = comesFromRight ? spawnPosRight.position : spawnPosLeft.position;
        WindGust gust = Instantiate(windGustPrefab, spawnPosition, Quaternion.LookRotation(direction));
        gust.Initialize(direction, player);
    }
}
