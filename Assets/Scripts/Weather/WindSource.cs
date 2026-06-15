using System;
using UnityEngine;

public class WindSource : MonoBehaviour
{
    public event Action OnPulseStarted;
    public float Strength => config.strength;
    public bool IsGlobal => config.globalWind;
    public float Radius => config.radius;
    public bool IsActive
    {
        get
        {
            if (!config.pulseMode)
                return true;
            return isPulseActive;
        }
    }
    public Vector3 Direction
    {
        get
        {
            if (config.useTransformDirection)
                return (transform.forward * 0.4f + Vector3.down).normalized;
            return config.direction.normalized;
        }
    }

    [SerializeField] private WindSourceConfig config;

    private bool isPulseActive = true;
    private float pulseTimer;

    private void OnEnable()
    {
        WindManager.Register(this);
    }

    private void Update()
    {
        if (!config.pulseMode)
            return;

        pulseTimer += Time.deltaTime;
        if (isPulseActive)
        {
            if (pulseTimer >= config.pulseDuration)
            {
                isPulseActive = false;
                pulseTimer = 0f;
            }
        }
        else
        {
            if (pulseTimer >= config.pauseDuration)
            {
                isPulseActive = true;
                pulseTimer = 0f;
                OnPulseStarted?.Invoke();
            }
        }
    }

    public void SetDirection(Vector3 newDirection)
    {
        config.direction = newDirection.normalized;
    }

    private void OnDisable()
    {
        WindManager.Unregister(this);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (!config.globalWind)
            Gizmos.DrawWireSphere(transform.position, config.radius);
        Gizmos.DrawRay(transform.position, Direction * 10f);
    }
#endif
}
