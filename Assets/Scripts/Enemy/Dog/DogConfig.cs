using UnityEngine;

[CreateAssetMenu(fileName = "DogConfig", menuName = "Game/Enemies/Dog Config")]
public class DogConfig : ScriptableObject
{
    public float patrolSpeed = 3f;
    public float chaseSpeed = 5f;
    public float scaredSpeed = 8f;
}
