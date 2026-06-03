using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Game/Player/Player Config")]
public class PlayerConfig : ScriptableObject
{
    public float throttleIncrement = 0.7f;
    public float maxThrust = 200f;
    public float lift = 135f;
    public float turnSpeed = 90f;
    public float pitchSpeed = 30f;
    public float maxPitchAngle = 40f;
}
