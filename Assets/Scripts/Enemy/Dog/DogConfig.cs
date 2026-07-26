using UnityEngine;

[CreateAssetMenu(fileName = "DogConfig", menuName = "Game/Enemies/Dog Config")]
public class DogConfig : ScriptableObject
{
    public float patrolSpeed = 3f;
    public float scaredSpeed = 8f;
    public float scaredDuration = 5f;
    public ThrowableData newspaper;
}
