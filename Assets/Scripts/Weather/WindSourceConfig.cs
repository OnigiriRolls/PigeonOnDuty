using UnityEngine;

[CreateAssetMenu(fileName = "WindSourceConfig", menuName = "Game/Enemies/Wind Source Config")]
public class WindSourceConfig : ScriptableObject
{
    public Vector3 direction = Vector3.forward;
    public bool useTransformDirection;
    public float strength = 10f;
    public float radius = 25f;
    public bool globalWind = false;
    public bool pulseMode = false;
    public float pulseDuration = 0.5f;
    public float pauseDuration = 1f;
}
