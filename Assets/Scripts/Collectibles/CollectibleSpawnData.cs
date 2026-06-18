using System;
using UnityEngine;

[Serializable]
public class CollectibleSpawnData
{
    public GameObject prefab;

    public CollectibleType type;

    [Range(0f, 1f)]
    public float spawnChance = 0.5f;
}
