using System.Collections;
using TMPro;
using UnityEngine;

public class WindAttackManager : MonoBehaviour
{
    [SerializeField] private WindGust windGustPrefab;
    [SerializeField] private PlayerController player;
    [SerializeField] private float attackIntervalMin = 10f;
    [SerializeField] private float attackIntervalMax = 20f;
    [SerializeField] private int minGusts = 2;
    [SerializeField] private int maxGusts = 5;
    [SerializeField] private float gustInterval = 1f;
    [SerializeField] private AudioClip[] windAttackClips;
    [SerializeField] private float warningDuration = 3f;
    [SerializeField] private Transform spawnPosLeft;
    [SerializeField] private Transform spawnPosRight;

    private float attackInterval;
    private bool attackRunning;
    private bool isActive;
    private Transform playerTransform;

    private void Start()
    {
        isActive = false;
        playerTransform = player.transform;
    }

    public void StartAttack()
    {
        if (isActive)
            return;
        isActive = true;
        attackRunning = false;
        attackInterval = Random.Range(attackIntervalMin, attackIntervalMax);
    }

    private void Update()
    {
        if (!isActive)
            return;
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
        WarningManager.Instance.Show("Strong Winds Incoming", "");
        float timer = warningDuration;
        while (timer > 0f)
        {
            WarningManager.Instance.SetContent(Mathf.CeilToInt(timer).ToString());
            timer -= Time.deltaTime;
            yield return null;
        }

        WarningManager.Instance.Hide();
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
            playerTransform.right, -playerTransform.right,
            (playerTransform.right + Vector3.up).normalized, (-playerTransform.right + Vector3.up).normalized,
            (playerTransform.right + Vector3.down).normalized, (-playerTransform.right + Vector3.down).normalized };

        Vector3 direction = directions[Random.Range(0, directions.Length)];
        bool comesFromRight = Vector3.Dot(direction, playerTransform.right) > 0f;
        Vector3 spawnPosition = comesFromRight ? spawnPosRight.position : spawnPosLeft.position;
        WindGust gust = Instantiate(windGustPrefab, spawnPosition, Quaternion.LookRotation(direction));
        float playerSpeed = player.Velocity.magnitude * 3.6f;
        float gustSpeed = windGustPrefab.Config.GetMoveSpeed(playerSpeed);
        gust.Initialize(direction, playerTransform, gustSpeed);
    }

    public void StopAttack()
    {
        isActive = false;
        attackRunning = false;
        StopAllCoroutines();
        WarningManager.Instance.Hide();
        AudioManager.Instance.StopEnvironmentalLoop();
    }
}
