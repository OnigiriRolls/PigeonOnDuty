using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParadeZoneController : MonoBehaviour
{
    public bool IsDispersing { get; private set; }

    [SerializeField] private NPCParadePerson[] personPrefabs;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private Transform spawnPointsParent;
    [SerializeField] private Transform spawnNPCsParent;
    [SerializeField] private int minPeople = 5;
    [SerializeField] private int maxPeople = 12;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clips;

    private readonly List<NPCParadePerson> spawnedPeople = new();
    private ParadeManager paradeManager;

    private void Start()
    {
        paradeManager = FindAnyObjectByType<ParadeManager>();
        paradeManager.RegisterParade(this);
    }

    public void SpawnParade()
    {
        AudioManager.Instance.PlayRandomSFX(clips, audioSource);
        Transform[] spawnPoints = GetSpawnPoints();
        int count = Random.Range(minPeople, maxPeople + 1);
        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            NPCParadePerson prefab = personPrefabs[Random.Range(0, personPrefabs.Length)];
            NPCParadePerson person = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation, spawnNPCsParent);
            person.Initialize(centerPoint.position, spawnPoint.position);
            spawnedPeople.Add(person);
        }
    }

    private Transform[] GetSpawnPoints()
    {
        Transform[] result = new Transform[spawnPointsParent.childCount];
        for (int i = 0; i < spawnPointsParent.childCount; i++)
            result[i] = spawnPointsParent.GetChild(i);
        return result;
    }

    public void DisperseParade()
    {
        if (IsDispersing)
            return;
        audioSource.Stop();
        IsDispersing = true;
        foreach (NPCParadePerson person in spawnedPeople)
        {
            if (person != null)
                person.ReturnHome();
        }
        StartCoroutine(WaitForParadeToFinish());
    }

    private IEnumerator WaitForParadeToFinish()
    {
        while (true)
        {
            bool allGone = true;
            foreach (NPCParadePerson person in spawnedPeople)
            {
                if (person != null)
                {
                    allGone = false;
                    break;
                }
            }
            if (allGone)
                break;
            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (paradeManager != null)
            paradeManager.UnregisterParade(this);
    }
}
