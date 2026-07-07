using UnityEngine;

[CreateAssetMenu(fileName = "WindCorridorConfig", menuName = "Game/Enemies/Wind Corridor Config")]
public class WindCorridorConfig : ScriptableObject
{
    public float lifetime = 12f;
    public float speedMultiplier = 1.3f;
    public float spawnDistance = 120f;
    public float minSpawnTime = 15f;
    public float maxSpawnTime = 30f;
    public float horizontalOffset = 15f;
    public float verticalOffset = 8f;
}
