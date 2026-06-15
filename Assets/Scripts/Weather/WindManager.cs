using System.Collections.Generic;
using UnityEngine;

public class WindManager : MonoBehaviour
{
    public static WindManager Instance { get; private set; }

    private static readonly List<WindSource> windSources = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public static void Register(WindSource source)
    {
        if (!windSources.Contains(source))
            windSources.Add(source);
    }

    public static void Unregister(WindSource source)
    {
        windSources.Remove(source);
    }

    public Vector3 GetWindAtPosition(Vector3 position)
    {
        Vector3 totalWind = Vector3.zero;
        foreach (WindSource source in windSources)
        {
            if (source == null)
                continue;
            if (!source.IsActive)
                continue;
            if (source.IsGlobal)
            {
                totalWind += source.Direction * source.Strength;
                continue;
            }
            float distance = Vector3.Distance(position, source.transform.position);
            if (distance > source.Radius)
                continue;
            float falloff = 1f - (distance / source.Radius);
            totalWind += falloff * source.Strength * source.Direction;
        }
        return totalWind;
    }
}
